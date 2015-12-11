using System;
using System.IO;
using System.Threading.Tasks;

namespace VkTunes.Api
{
    public interface IVk
    {
        Task<UserAudioResponse> MyAudio();

        Task<long?> GetFileSize(string url);

        Task<RemoteAudioRecord> DownloadAudioFileTo(Stream stream, int audioId, int owner, IProgress<AudioDownloadProgress> progress);

        void CancelTasks(int priority);
    }
}