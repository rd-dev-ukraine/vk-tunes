using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

using VkTunes.Api.Client.Audio;
using VkTunes.Api.Infrastructure.Http;
using VkTunes.Api.Infrastructure.Queue;

namespace VkTunes.Api.Client
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

        public Task<RemoteAudioRecord[]> GetAudioById(int audioId, int ownerId)
        {
            var request = new GetAudioByIdRequest { AudioId = $"{ownerId}_{audioId}" };
            return apiClient.CallApi<GetAudioByIdRequest, RemoteAudioRecord[]>("audio.getById", request);
        }

        public Task<long?> FileSize(string url)
        {
            return apiClient.FileSize(url);
        }

        public async Task<RemoteAudioRecord> DownloadTo(Stream stream, int audioId, int owner, IProgress<AudioDownloadProgress> progress)
        {
            if (stream == null)
                throw new ArgumentNullException(nameof(stream));

            var audioInfo = await methodScopeQueue.Enqueue(() => GetAudioById(audioId, owner));
            var audio = audioInfo?.FirstOrDefault();
            if (audio == null)
                throw new ArgumentException("Audio record not found");

            return await methodScopeQueue.Enqueue(async () =>
            {

                await apiClient.DowloadTo(stream, audio.FileUrl, progress);
                return audio;
            });
        }
    }
}