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
        private readonly IVkApiClient apiClient;
        /// <summary>
        /// Internal request queue. Should be used only in case of few API calls inside single method.
        /// </summary>
        private readonly VkRequestQueue methodScopeQueue = new VkRequestQueue();

        public Vk(IVkApiClient apiClient)
        {
            if (apiClient == null)
                throw new ArgumentNullException(nameof(apiClient));

            this.apiClient = apiClient;
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

            var audio = await methodScopeQueue.Enqueue(() => GetAudioById(audioId, owner));

            return await methodScopeQueue.Enqueue(async () =>
            {
                await apiClient.DownloadTo(stream, audio.FileUrl, progress);
                return audio;
            });
        }
    }
}