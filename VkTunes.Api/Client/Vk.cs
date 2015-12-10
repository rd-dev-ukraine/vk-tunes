using System;
using System.IO;
using System.Threading.Tasks;

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
            return CallApi<UserAudioResponse>("audio.get");
        }

        public Task<long?> FileSize(string url)
        {
            return queue.Enqueue(() => apiClient.FileSize(url));
        }

        public Task<long?> FileSizePriore(string url)
        {
            return queue.EnqueuePriore(() => apiClient.FileSize(url));
        }

        public async Task DownloadTo(Stream stream, string fileUrl, IProgress<AudioDownloadProgress> progress)
        {
            if (stream == null)
                throw new ArgumentNullException(nameof(stream));
            if (String.IsNullOrWhiteSpace(fileUrl))
                throw new ArgumentNullException(nameof(fileUrl));

            await queue.EnqueuePriore(async () =>
            {
                await apiClient.DowloadTo(stream, fileUrl, progress);
                return true;
            });
        }

        private Task<TResponse> CallApi<TResponse>(string method)
            where TResponse : class
        {
            queue.Clear();
            return queue.Enqueue(() => apiClient.CallApi<TResponse>(method));
        }
    }
}