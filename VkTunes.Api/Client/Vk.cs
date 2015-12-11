using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

using VkTunes.Api.LowLevel;
using VkTunes.Api.Models;
using VkTunes.Api.Queue;

namespace VkTunes.Api
{
    public class Vk : IVk
    {
        /// <summary>
        /// Puts the API call to the first position of queue - task will be execute immediately after delay passed.
        /// </summary>
        private readonly IVkApiClient apiClient;

        /// <summary>
        /// Internal request queue. Should be used only in case of few API calls inside single method.
        /// </summary>
        private readonly IApiRequestQueue requestQueue;

        public Vk(IVkApiClient apiClient, IApiRequestQueue requestQueue)
        {
            if (apiClient == null)
                throw new ArgumentNullException(nameof(apiClient));
            if (requestQueue == null)
                throw new ArgumentNullException(nameof(requestQueue));

            this.apiClient = apiClient;
            this.requestQueue = requestQueue;
        }

        public Task<UserAudioResponse> MyAudio()
        {
            return requestQueue.EnqueueFirst(() => apiClient.CallApi<UserAudioResponse>("audio.get"), QueuePriorities.ApiCall, "API::audio.get");
        }

        public async Task<RemoteAudioRecord> GetAudioById(int audioId, int ownerId)
        {
            var request = new GetAudioByIdRequest { AudioId = $"{ownerId}_{audioId}" };

            var all = await requestQueue.EnqueueFirst(
                                () => apiClient.CallApi<GetAudioByIdRequest, RemoteAudioRecord[]>("audio.getById", request),
                                QueuePriorities.ApiCall,
                                $"API::audio.getById({ownerId}_{audioId})");


            var audio = all.FirstOrDefault();
            if (audio == null)
                throw new VkApiCallException($"Call audio.getById(audioId:{audioId}, owner:{ownerId}) doesn't return an audio.");

            return audio;
        }

        public Task<long?> GetFileSize(string url)
        {
            return requestQueue.EnqueueLast(() => apiClient.GetFileSize(url), QueuePriorities.GetFileSize, $"Get file size for file {url}");
        }

        public async Task<RemoteAudioRecord> DownloadAudioFileTo(Stream stream, int audioId, int owner, IProgress<AudioDownloadProgress> progress)
        {
            if (stream == null)
                throw new ArgumentNullException(nameof(stream));
            

            return await requestQueue.EnqueueFirst(async () =>
                                        {
                                            // Error: unable to enqueue tasks inside other queued task
                                            var audio = await GetAudioById(audioId, owner);
                                            await apiClient.DownloadTo(stream, audio.FileUrl, progress);
                                            return audio;
                                        }, 
                                        QueuePriorities.DownloadFile,
                                        $"Download file for audio {owner}_{audioId}");
        }

        public void CancelTasks(int priority)
        {
            requestQueue.Clear(priority);
        }
    }
}