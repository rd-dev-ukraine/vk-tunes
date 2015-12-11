using System;
using System.Threading.Tasks;

using VkTunes.Api.Api;
using VkTunes.Api.AudioStorage;

namespace VkTunes.Api.Models
{
    public class SearchAudioCollection : AudioCollectionBase
    {
        public SearchAudioCollection(Vk vk, IVkAudioFileStorage storage) 
            : base(vk, storage)
        {
        }

        protected override async Task<RemoteAudioRecord[]> GetAudio()
        {
            throw new NotImplementedException();
        }
    }
}