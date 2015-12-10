using System.CodeDom;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

using VkTunes.Api.Client.Audio;

namespace VkTunes.Api.AudioStorage
{
    public interface IVkAudioFileStorage
    {
        Task<Dictionary<int, LocalAudioRecord>> Load();

        Task Save(Stream source, RemoteAudioRecord audio);
    }
}