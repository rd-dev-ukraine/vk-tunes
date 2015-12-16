﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

using VkTunes.Api.Api;
using VkTunes.Api.AudioStorage;
using VkTunes.Api.Queue;
using VkTunes.Api.Utils;

namespace VkTunes.Api.Models.Collections
{
    /// <summary>
    /// A collection of audio record represented remote and local record (or both).
    /// Collection performs some background update of audio information and allows 
    /// to save audio to storage and add/remove it to VK.
    /// </summary>
    public abstract class AudioCollectionBase
    {
        private readonly bool includeOrphanLocalAudio;

        protected AudioCollectionBase(
            Vk vk,
            IVkAudioFileStorage storage,
            bool includeOrphanLocalAudio)
        {
            if (vk == null)
                throw new ArgumentNullException(nameof(vk));
            if (storage == null)
                throw new ArgumentNullException(nameof(storage));

            VK = vk;
            Storage = storage;
            this.includeOrphanLocalAudio = includeOrphanLocalAudio;

            Storage.LocalAudioUpdated += OnLocalAudioUpdated;
        }

        public ObservableCollection<AudioInfo> Audio { get; } = new ObservableCollection<AudioInfo>();

        protected Vk VK { get; }

        protected IVkAudioFileStorage Storage { get; }

        public async Task Reload()
        {
            var audio = await LoadAudioInfo();
            Audio.Clear();

            var audioInfos = audio as AudioInfo[] ?? audio.ToArray();
            foreach (var a in audioInfos)
                Audio.Add(a);


            foreach (var a in audioInfos.Where(r => r.RemoteAudio != null && !String.IsNullOrWhiteSpace(r.RemoteAudio.FileUrl)))
            {
                var audioInfo = a;
                VK.GetFileSize(audioInfo.RemoteAudio.FileUrl)
                    .ContinueWith(sizeResult =>
                    {
                        audioInfo.RemoteFileSize = sizeResult.Result;
                        AudioInfoUpdated?.Invoke(this, new AudioInfoUpdatedEventArgs
                        {
                            Audio = audioInfo,
                            AudioId = audioInfo.Id
                        });
                    })
                    .FireAndForget();
            }
        }

        public void CancelSizeLoading()
        {
            VK.CancelTasks(QueuePriorities.GetFileSize);
        }

        protected abstract Task<RemoteAudioRecord[]> GetAudio();

        private async Task<IEnumerable<AudioInfo>> LoadAudioInfo()
        {
            VK.CancelTasks(QueuePriorities.GetFileSize);

            var loadAudioTask = GetAudio();
            var loadStorageTask = Storage.Load();

            var data = await TaskUtils.WhenAll(loadAudioTask, loadStorageTask);
            var remoteAudio = data.Item1.ToDictionary(r => r.Id);
            var storedAudio = data.Item2;

            var allAudioId = new HashSet<int>();
            foreach (var remote in remoteAudio)
                allAudioId.Add(remote.Key);

            if (includeOrphanLocalAudio)
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