using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using VkTunes.Api.AudioStorage;
using VkTunes.Api.Queue;
using VkTunes.Api.Utils;

namespace VkTunes.Api.Models
{
    /// <summary>
    /// A collection of audio record represented remote and local record (or both).
    /// Collection performs some background update of audio information and allows 
    /// to save audio to storage and add/remove it to VK.
    /// </summary>
    public abstract class AudioCollectionBase
    {
        protected AudioCollectionBase(IVk vk, IVkAudioFileStorage storage, VkRequestQueue queue)
        {
            if (vk == null)
                throw new ArgumentNullException(nameof(vk));
            if (storage == null)
                throw new ArgumentNullException(nameof(storage));
            if (queue == null)
                throw new ArgumentNullException(nameof(queue));

            VK = vk;
            Storage = storage;
            Queue = queue;

            Storage.LocalAudioUpdated += OnLocalAudioUpdated;
        }

        public ObservableCollection<AudioInfo> Audio { get; } = new ObservableCollection<AudioInfo>();

        protected IVk VK { get; }

        protected IVkAudioFileStorage Storage { get; }

        protected VkRequestQueue Queue { get; }

        public async Task Reload()
        {
            var audio = await LoadAudioInfo();
            SynchronizationContext.Current.Send(_ =>
            {
                Audio.Clear();
                foreach (var a in audio)
                    Audio.Add(a);

            }, null);

            foreach (var a in audio.Where(r => r.RemoteAudio != null))
            {
                var audioInfo = a;
                audioInfo.RemoteFileSize = await Queue.Enqueue(() => VK.FileSize(audioInfo.RemoteAudio.FileUrl));

                SynchronizationContext.Current.Send(_ =>
                {
                    AudioInfoUpdated?.Invoke(this, new AudioInfoUpdatedEventArgs
                    {
                        Audio = audioInfo,
                        AudioId = audioInfo.Id
                    });
                }, null);
            }
        }

        protected abstract Task<RemoteAudioRecord[]> GetAudio();

        private async Task<IEnumerable<AudioInfo>> LoadAudioInfo()
        {
            Queue.Clear();

            var loadAudioTask = Queue.EnqueuePriore(GetAudio);
            var loadStorageTask = Storage.Load();

            var data = await TaskUtils.WhenAll(loadAudioTask, loadStorageTask);
            var remoteAudio = data.Item1.ToDictionary(r => r.Id);
            var storedAudio = data.Item2;

            var allAudioId = new HashSet<int>();
            foreach (var remote in remoteAudio)
                allAudioId.Add(remote.Key);
            foreach (var local in storedAudio)
                allAudioId.Add(local.Key);

            var result = new List<AudioInfo>();
            foreach (var id in allAudioId)
            {
                LocalAudioRecord local;
                RemoteAudioRecord remote;

                storedAudio.TryGetValue(id, out local);
                remoteAudio.TryGetValue(id, out remote);

                result.Add(new AudioInfo
                {
                    Id = id,
                    LocalAudio = local,
                    RemoteAudio = remote
                });
            }

            return result;
        }

        private void OnLocalAudioUpdated(object sender, LocalAudioRecordUpdatedEventArgs e)
        {
            var audio = Audio.FirstOrDefault(a => a.Id == e.AudioId);
            if (audio != null)
            {
                audio.LocalAudio = e.LocalAudio;
                AudioInfoUpdated?.Invoke(this, new AudioInfoUpdatedEventArgs { Audio = audio, AudioId = audio.Id });
            }
        }

        public event EventHandler<AudioInfoUpdatedEventArgs> AudioInfoUpdated;
    }
}