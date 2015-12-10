using System;
using System.IO;
using System.Threading.Tasks;

namespace VkTunes.Api.Client
{
    public interface IVk
    {
        Task<UserAudioResponse> MyAudio();

        Task<long?> FileSize(string url);

        Task DownloadTo(Stream stream, string fileUrl, IProgress<AudioDownloadProgress> progress);

        Task<long?> FileSizePriore(string url);
    }
}