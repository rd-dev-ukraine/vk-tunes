using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace VkTunes.Api.AudioStorage
{
    public interface IVkAudioFileStorage
    {
        Task<Dictionary<int, StoredAudioRecord>> Load();

        string GenerateFileName(int audioId, string artist, string title);

        Stream OpenSave(string fileName);
    }
}