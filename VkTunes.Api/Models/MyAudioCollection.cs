using System.Threading.Tasks;

using VkTunes.Api.AudioStorage;
using VkTunes.Api.Client;
using VkTunes.Api.Client.Audio;
using VkTunes.Api.Infrastructure.Queue;

namespace VkTunes.Api.Models
{
    public class MyAudioCollection : AudioCollectionBase
    {
        public MyAudioCollection(IVk vk, IVkAudioFileStorage storage, VkRequestQueue queue) 
            : base(vk, storage, queue)
        {
        }

        protected override async Task<RemoteAudioRecord[]> GetAudio()
        {
            var r = await VK.MyAudio();
            return r.Audio;
        }
    }
}