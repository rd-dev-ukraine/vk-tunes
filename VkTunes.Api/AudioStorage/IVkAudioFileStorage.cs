using System;
using System.CodeDom;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace VkTunes.Api.AudioStorage
{
    public interface IVkAudioFileStorage
    {
        Task<Dictionary<int, LocalAudioRecord>> Load();

        Task Save(Stream source, RemoteAudioRecord audio);

        event EventHandler<LocalAudioRecordUpdatedEventArgs> LocalAudioUpdated;
    }
}