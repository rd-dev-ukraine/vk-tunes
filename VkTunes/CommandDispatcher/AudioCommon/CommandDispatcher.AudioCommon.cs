using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using VkTunes.Api.Api;
using VkTunes.Api.AudioStorage;
using VkTunes.Api.Models;
using VkTunes.Api.Queue;
using VkTunes.Api.Utils;
using VkTunes.CommandDispatcher.GetFileSize;

// ReSharper disable once CheckNamespace
namespace VkTunes.CommandDispatcher
{
    public partial class CommandDispatcher
    {
        private async Task<IEnumerable<AudioInfo>> LoadAudioCollection(Func<Task<RemoteAudioRecord[]>> audioSet)
        {
            vk.CancelTasks(QueuePriorities.GetFileSize);

            var loadAudioTask = audioSet();
            var loadStorageTask = storage.Load();

            var data = await TaskUtils.WhenAll(loadAudioTask, loadStorageTask);
            var remoteAudio = data.Item1.ToDictionary(r => r.Id);
            var storedAudio = data.Item2;

            var allAudioId = new HashSet<int>();
            foreach (var remote in remoteAudio)
                allAudioId.Add(remote.Key);

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

            foreach (var audio in result.Where(a => !String.IsNullOrWhiteSpace(a.RemoteAudio?.FileUrl)))
                Command(new UpdateRemoteFileSizeCommand(audio.Id, audio.RemoteAudio.Owner, audio.RemoteAudio.FileUrl));

            return result;
        }
    }
}