using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

using VkTunes.Api.LowLevel;
using VkTunes.Api.Queue;

namespace VkTunes.Api
{
    public class Vk : IVk
    {
        /// <summary>
        /// Puts the API call to the first position of queue - task will be execute immediately after delay passed.
        /// </summary>
        private const int QueuePriorityImmediate = 10000;
        private readonly IVkApiClient apiClient;

        /// <summary>
        /// Internal request queue. Should be used only in case of few API calls inside single method.
        /// </summary>
        private readonly IApiRequestQueue methodScopeQueue;

        public Vk(IVkApiClient apiClient, IApiRequestQueue methodScopeQueue)
        {
            if (apiClient == null)
                throw new ArgumentNullException(nameof(apiClient));
            if (methodScopeQueue == null)
                throw new ArgumentNullException(nameof(methodScopeQueue));

            this.apiClient = apiClient;
            this.methodScopeQueue = methodScopeQueue;
        }

        public Task<UserAudioResponse> MyAudio()
        {
            return apiClient.CallApi<UserAudioResponse>("audio.get");
        }

        public async Task<RemoteAudioRecord> GetAudioById(int audioId, int ownerId)
        {
            var request = new GetAudioByIdRequest { AudioId = $"{ownerId}_{audioId}" };
            var all = await apiClient.CallApi<GetAudioByIdRequest, RemoteAudioRecord[]>("audio.getById", request);
            var audio = all.FirstOrDefault();
            if (audio == null)
                throw new VkApiCallException($"Call audio.getById(audioId:{audioId}, owner:{ownerId}) doesn't return an audio.");

            return audio;
        }

        public Task<long?> GetFileSize(string url)
        {
            return apiClient.GetFileSize(url);
        }

        public async Task<RemoteAudioRecord> DownloadAudioFileTo(Stream stream, int audioId, int owner, IProgress<AudioDownloadProgress> progress)
        {
            if (stream == null)
                throw new ArgumentNullException(nameof(stream));

            var audio = await methodScopeQueue.EnqueueFirst(() => GetAudioById(audioId, owner), QueuePriorityImmediate);

            return await methodScopeQueue.EnqueueFirst(async () =>
            {
                await apiClient.DownloadTo(stream, audio.FileUrl, progress);
                return audio;
            }, QueuePriorityImmediate);
        }
    }
}