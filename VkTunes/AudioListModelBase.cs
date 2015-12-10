using System;
using System.Linq;
using System.Threading.Tasks;

using Caliburn.Micro;

using VkTunes.Api.Models;
using VkTunes.AudioRecord;

namespace VkTunes
{
    public abstract class AudioListModelBase : Screen
    {
        protected AudioListModelBase(AudioCollectionBase audioCollection)
        {
            AudioCollection = audioCollection;
            AudioCollection.Audio.CollectionChanged += (sender, e) =>
            {
                Audio.Clear();
                Audio.AddRange(AudioCollection.Audio.Select(r => Map(r)));
            };
            AudioCollection.AudioInfoUpdated += (s, e) =>
            {
                var model = Audio.SingleOrDefault(a => a.Id == e.AudioId);
                if (model != null)
                    Map(e.Audio, model);
            };
        }

        public BindableCollection<AudioRecordViewModel> Audio { get; set; } = new BindableCollection<AudioRecordViewModel>();

        protected AudioCollectionBase AudioCollection { get; }

        public async Task Reload()
        {
            await AudioCollection.Reload();
        }

        private AudioRecordViewModel Map(AudioInfo source, AudioRecordViewModel dest = null)
        {
            dest = dest ?? new AudioRecordViewModel();

            dest.Id = source.Id;
            dest.Duration = TimeSpan.FromSeconds(source.RemoteAudio?.DurationInSeconds ?? 0);
            dest.Title = source.RemoteAudio == null ? source.LocalAudio.Name : $"{source.RemoteAudio?.Artist} - {source.RemoteAudio?.Title}";
            dest.IsInStorage = source.LocalAudio != null;
            dest.IsInVk = source.RemoteAudio != null;
            dest.LocalFilePath = source.LocalAudio?.FilePath;
            dest.FileSize = source.RemoteFileSize;

            return dest;
        }
    }
}