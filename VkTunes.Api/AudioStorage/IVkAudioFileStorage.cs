using System.Collections.Generic;
using System.Threading.Tasks;

namespace VkTunes.Api.AudioStorage
{
    public interface IVkAudioFileStorage
    {
        Task<Dictionary<int, StoredAudioRecord>> Load();
    }
}