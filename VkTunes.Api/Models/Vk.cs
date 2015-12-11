using System;
using System.IO;
using System.Threading.Tasks;

using VkTunes.Api.Api;
using VkTunes.Api.Queue;

namespace VkTunes.Api.Models
{
    public class Vk
    {
        /// <summary>
        /// Puts the API call to the first position of queue - task will be execute immediately after delay passed.
        /// </summary>
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

        public Task<long?> GetFileSize(string url)
        {
            return queue.EnqueueLast(() => api.GetFileSize(url), QueuePriorities.GetFileSize, $"Get file size for file {url}");
        }

        public async Task<RemoteAudioRecord> DownloadAudioFileTo(Stream stream, int audioId, int owner, IProgress<AudioDownloadProgress> progress)
        {
            if (stream == null)
                throw new ArgumentNullException(nameof(stream));

            return await queue.EnqueueFirst(() => api.DownloadAudioFileTo(stream, audioId, owner, progress),
                                    QueuePriorities.DownloadFile,
                                    $"Download file for audio {owner}_{audioId}");
        }

        public void CancelTasks(int priority)
        {
            queue.Clear(priority);
        }
    }
}