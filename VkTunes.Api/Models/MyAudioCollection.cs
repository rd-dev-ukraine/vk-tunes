using System.Threading.Tasks;

using VkTunes.Api.AudioStorage;
using VkTunes.Api.Client;
using VkTunes.Api.Client.Audio;

namespace VkTunes.Api.Models
{
    public class MyAudioCollection : AudioCollectionBase
    {
        public MyAudioCollection(IVk vk, IVkAudioFileStorage storage) 
            : base(vk, storage)
        {
        }

        protected override async Task<RemoteAudioRecord[]> GetAudio()
        {
            var r = await VK.MyAudio();
            return r.Audio;
        }
    }
}