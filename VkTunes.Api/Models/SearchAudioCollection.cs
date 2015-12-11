using System;
using System.Threading.Tasks;

using VkTunes.Api.Api;
using VkTunes.Api.AudioStorage;
using VkTunes.Api.Utils;

namespace VkTunes.Api.Models
{
    public class SearchAudioCollection : AudioCollectionBase
    {
        private string query;

        public SearchAudioCollection(Vk vk, IVkAudioFileStorage storage) 
            : base(vk, storage)
        {
        }

        public string Query
        {
            get { return query; }
            set
            {
                if (query != value)
                {
                    query = value;
                    VK.CancelTasks(QueuePriorities.ApiCallSearchAudio);
                    VK.CancelTasks(QueuePriorities.GetFileSize);
                    Reload().FireAndForget();
                }
            }
        }

        protected override async Task<RemoteAudioRecord[]> GetAudio()
        {
            var result = await VK.SearchAudio(Query);
            return result.Audio;
        }
    }
}