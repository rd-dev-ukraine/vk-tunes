using System.Threading.Tasks;

using VkTunes.Api.Api;
using VkTunes.Api.AudioStorage;

namespace VkTunes.Api.Models
{
    public class MyAudioCollection : AudioCollectionBase
    {
        public MyAudioCollection(Vk vk, IVkAudioFileStorage storage) 
            : base(vk, storage, true)
        {
        }

        protected override async Task<RemoteAudioRecord[]> GetAudio()
        {
            var r = await VK.MyAudio();
            return r.Audio;
        }
    }
}