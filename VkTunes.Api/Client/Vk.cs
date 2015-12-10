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
        private readonly VkRequestQueue queue = new VkRequestQueue();

        public Vk(IVkApiClient apiClient)
        {
            if (apiClient == null)
                throw new ArgumentNullException(nameof(apiClient));

            this.apiClient = apiClient;
        }

        public Task<UserAudioResponse> MyAudio()
        {
            return queue.EnqueuePriore(() => apiClient.CallApi<UserAudioResponse>("audio.get"));
        }

        public Task<RemoteAudioRecord[]> GetAudioById(int audioId, int ownerId)
        {
            var request = new GetAudioByIdRequest { AudioId = $"{ownerId}_{audioId}" };
            return queue.EnqueuePriore(() => apiClient.CallApi<GetAudioByIdRequest, RemoteAudioRecord[]>("audio.getById", request));
        }

        public Task<long?> FileSize(string url)
        {
            return queue.Enqueue(() => apiClient.FileSize(url));
        }

        public Task<long?> FileSizePriore(string url)
        {
            return queue.EnqueuePriore(() => apiClient.FileSize(url));
        }

        public async Task<RemoteAudioRecord> DownloadTo(Stream stream, int audioId, int owner, IProgress<AudioDownloadProgress> progress)
        {
            if (stream == null)
                throw new ArgumentNullException(nameof(stream));

            var audioInfo = await GetAudioById(audioId, owner);
            var audio = audioInfo?.FirstOrDefault();
            if (audio == null)
                throw new ArgumentException("Audio record not found");

            return await queue.EnqueuePriore(async () =>
            {

                await apiClient.DowloadTo(stream, audio.FileUrl, progress);
                return audio;
            });
        }
    }
}