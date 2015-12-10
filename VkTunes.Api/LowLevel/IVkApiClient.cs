using System;
using System.IO;
using System.Threading.Tasks;

namespace VkTunes.Api.LowLevel
{
    /// <summary>
    /// Generic low-level access to vk.com API.
    /// </summary>
    public interface IVkApiClient
    {
        Task<TResponse> CallApi<TRequest, TResponse>(string apiMethod, TRequest request) 
            where TRequest: class
            where TResponse: class;

        Task<TResponse> CallApi<TResponse>(string apiMethod)
            where TResponse : class;

        Task<long?> GetFileSize(string url);

        Task DownloadTo(Stream stream, string fileUrl, IProgress<AudioDownloadProgress> progress = null);
    }
}