using System;
using System.Linq;
using System.Threading.Tasks;

using Caliburn.Micro;

using VkTunes.Api.Models.Collections;
using VkTunes.AudioRecord;
using VkTunes.DownloadProgress;

namespace VkTunes
{
    public abstract class AudioListModelBase<TAudioCollection> : Screen
        where TAudioCollection : AudioCollectionBase
    {
        private readonly IEventAggregator eventAggregator;

        protected AudioListModelBase(TAudioCollection audioCollection, IEventAggregator eventAggregator)
        {
            AudioCollection = audioCollection;
            this.eventAggregator = eventAggregator;
            AudioCollection.Audio.CollectionChanged += (sender, e) =>
            {
                Audio.Clear();
                Audio.AddRange(AudioCollection.Audio.Select(r => Map(r)));
            };
            AudioCollection.AudioInfoUpdated += (s, e) =>
            {
                Execute.OnUIThread(() =>
                {
                    var model = Audio.SingleOrDefault(a => a.Id == e.AudioId);
                    if (model != null)
                        Map(e.Audio, model);
                });
            };
        }

        protected override void OnDeactivate(bool close)
        {
            base.OnDeactivate(close);
            AudioCollection.CancelSizeLoading();
        }

        public BindableCollection<AudioRecordViewModel> Audio { get; set; } = new BindableCollection<AudioRecordViewModel>();

        protected TAudioCollection AudioCollection { get; }

        public async Task Reload()
        {
            await AudioCollection.Reload();
        }

        public void DownloadAll()
        {
            foreach (var audio in Audio)
                DownloadAudio(audio);
        }

        protected void DownloadAudio(AudioRecordViewModel audio)
        {
            if (audio.Id != 0 && audio.OwnerId != 0)
                eventAggregator.PublishOnBackgroundThread(new DownloadAudioEvent(audio.Id, audio.OwnerId));
        }

        private AudioRecordViewModel Map(AudioInfo source, AudioRecordViewModel dest = null)
        {
            dest = dest ?? new AudioRecordViewModel(eventAggregator);

            dest.Id = source.Id;
            dest.Duration = TimeSpan.FromSeconds(source.RemoteAudio?.DurationInSeconds ?? 0);
            dest.Artist = source.RemoteAudio?.Artist;
            dest.Title = source.RemoteAudio?.Title;
            dest.IsInStorage = source.LocalAudio != null;
            dest.IsInVk = source.RemoteAudio != null;
            dest.LocalFilePath = source.LocalAudio?.FilePath;
            dest.FileSize = source.RemoteFileSize;
            dest.OwnerId = source.RemoteAudio?.Owner ?? 0;

            return dest;
        }
    }
}