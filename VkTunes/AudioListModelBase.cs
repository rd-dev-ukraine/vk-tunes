using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Caliburn.Micro;

using VkTunes.Api.AudioStorage;
using VkTunes.Api.Client;
using VkTunes.AudioRecord;
using VkTunes.Infrastructure.Async;
using VkTunes.Utils;

namespace VkTunes
{
    public abstract class AudioListModelBase : Screen
    {
        protected AudioListModelBase(IVk vk, IVkAudioFileStorage storage, IAsync @async)
        {
            if (vk == null)
                throw new ArgumentNullException(nameof(vk));
            if (storage == null)
                throw new ArgumentNullException(nameof(storage));
            if (async == null)
                throw new ArgumentNullException(nameof(async));

            Vk = vk;
            Async = async;
            Storage = storage;
        }

        protected IVk Vk { get; }

        protected IAsync Async { get; }

        protected IVkAudioFileStorage Storage { get; }

        public BindableCollection<AudioRecordViewModel> Audio { get; set; } = new BindableCollection<AudioRecordViewModel>();

        protected abstract Task<UserAudioResponse> LoadAudio();

        public void Reload()
        {
            Async.Execute(LoadAudioInfo, r =>
            {
                Audio.Clear();
                foreach (var record in r)
                {
                    var model = Map(record);
                    Audio.Add(model);

                    if (record.RemoteAudio != null)
                        Async.Execute(() => Vk.FileSize(record.RemoteAudio.FileUrl), size => model.FileSize = size);
                }
            });
        }

        private AudioRecordViewModel Map(AudioTuple record)
        {
            return new AudioRecordViewModel
            {
                Id = record.AudioId,
                Duration = TimeSpan.FromSeconds(record.RemoteAudio?.DurationInSeconds ?? 0),
                Title = record.RemoteAudio == null ? record.LocalAudio.Name : $"{record.RemoteAudio?.Artist} - {record.RemoteAudio?.Title}",
                IsInStorage = record.LocalAudio != null,
                IsInVk = record.RemoteAudio != null,
                LocalFilePath = record.LocalAudio?.FilePath
            };
        }

        private async Task<IEnumerable<AudioTuple>> LoadAudioInfo()
        {
            var data = await TaskUtils.WhenAll(LoadAudio(), Storage.Load());
            var remoteAudio = data.Item1.Audio.ToDictionary(r => r.Id);
            var storedAudio = data.Item2;

            var result = new List<AudioTuple>();


            var allAudioId = new HashSet<int>();
            foreach (var remote in remoteAudio)
                allAudioId.Add(remote.Key);
            foreach (var local in storedAudio)
                allAudioId.Add(local.Key);


            foreach (var id in allAudioId)
            {
                StoredAudioRecord stored;
                Api.Client.AudioRecord remote;

                storedAudio.TryGetValue(id, out stored);
                remoteAudio.TryGetValue(id, out remote);

                result.Add(new AudioTuple
                {
                    AudioId = id,
                    LocalAudio = stored,
                    RemoteAudio = remote
                });
            }

            return result;
        }

        private class AudioTuple
        {
            public int AudioId { get; set; }

            public Api.Client.AudioRecord RemoteAudio { get; set; }

            public StoredAudioRecord LocalAudio { get; set; }
        }
    }
}