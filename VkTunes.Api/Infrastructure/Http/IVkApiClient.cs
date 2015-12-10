using System;
using System.IO;
using System.Threading.Tasks;

using VkTunes.Api.Client;

namespace VkTunes.Api.Infrastructure.Http
{
    /// <summary>
    /// Untyped access to vk.com API
    /// </summary>
    public interface IVkApiClient
    {
        Task<TResponse> CallApi<TRequest, TResponse>(string apiMethod, TRequest request) 
            where TRequest: class
            where TResponse: class;

        Task<TResponse> CallApi<TResponse>(string apiMethod)
            where TResponse : class;

        Task<long?> FileSize(string url);

        Task DowloadTo(Stream stream, string fileUrl, IProgress<AudioDownloadProgress> progress = null);
    }
}