using System;
using System.IO;
using System.Threading.Tasks;

namespace VkTunes.Api
{
    public interface IVk
    {
        Task<UserAudioResponse> MyAudio();

        Task<RemoteAudioRecord> GetAudioById(int audioId, int ownerId);

        Task<long?> FileSize(string url);

        Task<RemoteAudioRecord> DownloadTo(Stream stream, int audioId, int owner, IProgress<AudioDownloadProgress> progress);
    }
}