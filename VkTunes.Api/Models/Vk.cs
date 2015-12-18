using System;
using System.IO;
using System.Threading.Tasks;

using VkTunes.Api.Api;
using VkTunes.Api.Queue;

namespace VkTunes.Api.Models
{
    public class Vk
    {
        private readonly IVkApi api;
        private readonly IApiRequestQueue queue;

        public Vk(IVkApi api, IApiRequestQueue queue)
        {
            if (api == null)
                throw new ArgumentNullException(nameof(api));
            if (queue == null)
                throw new ArgumentNullException(nameof(queue));


            this.api = api;
            this.queue = queue;
        }

        public Task<UserAudioResponse> MyAudio()
        {
            return queue.EnqueueFirst(() => api.MyAudio(), QueuePriorities.ApiCall, "API::audio.get");
        }

        public Task<SearchAudioResponse> SearchAudio(string query)
        {
            return queue.EnqueueFirst(() => api.SearchAudio(query), QueuePriorities.ApiCallSearchAudio, "API::audio.search");
        }

        public Task<long?> GetFileSize(string url, bool asap)
        {
            Func<Task<long?>> workload = () => api.GetFileSize(url);
            var priority = QueuePriorities.GetFileSize;
            var description = $"Get file size for file {url}";

            return asap ? queue.EnqueueFirst(workload, priority, description) : queue.EnqueueLast(workload, priority, description);
        }

        public async Task<RemoteAudioRecord> DownloadAudioFileTo(Stream stream, int audioId, int owner, IProgress<AudioDownloadProgress> progress)
        {
            if (stream == null)
                throw new ArgumentNullException(nameof(stream));

            return await queue.EnqueueFirst(() => api.DownloadAudioFileTo(stream, audioId, owner, progress),
                                    QueuePriorities.DownloadFile,
                                    $"Download file for audio {owner}_{audioId}");
        }

        public async Task<RemoteAudioRecord> AddAudio(int audioId, int ownerId)
        {
            if (ownerId == api.MyUserId)
                return null;

            return await queue.EnqueueFirst(async () =>
            {
                var existing = await api.GetAudioByIdOrDefault(audioId, ownerId);
                if (existing != null)
                {
                    var addedAudio = await api.AddAudio(audioId, ownerId);
                    return await api.GetAudioById(addedAudio, api.MyUserId);
                }

                return default(RemoteAudioRecord);
            },
            QueuePriorities.ApiCall,
            $"Add audio {ownerId}_{audioId}");
        }

        public void CancelTasks(int priority)
        {
            queue.Clear(priority);
        }
    }
}