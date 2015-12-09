using System;
using System.Threading.Tasks;

using VkTunes.Api.Infrastructure.Http;

namespace VkTunes.Api.Client
{
    public class Vk : IVk
    {
        private readonly IVkApiClient apiClient;

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
    }
}