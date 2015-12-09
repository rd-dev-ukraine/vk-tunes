using System;
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

        private Task<TResponse> CallApi<TResponse>(string method)
            where TResponse : class
        {
            return queue.Enqueue(() => apiClient.CallApi<TResponse>(method));
        }
    }
}