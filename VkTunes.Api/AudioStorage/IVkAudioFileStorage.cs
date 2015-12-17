using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

using VkTunes.Api.Api;

namespace VkTunes.Api.AudioStorage
{
    public interface IVkAudioFileStorage
    {
        Task<Dictionary<int, LocalAudioRecord>> Load();

        Task<LocalAudioRecord> Save(Stream source, RemoteAudioRecord audio);
    }
}