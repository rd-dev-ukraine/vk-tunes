﻿using System;
using System.IO;
using System.Threading.Tasks;

using VkTunes.Api.Api;

namespace VkTunes.Api.LowLevel
{
    /// <summary>
    /// Generic low-level access to vk.com API.
    /// </summary>
    public interface IVkHttpClient
    {
        Task<TResponse> CallApi<TRequest, TResponse>(string apiMethod, TRequest request) ;

        Task<TResponse> CallApi<TResponse>(string apiMethod);

        Task<long?> GetFileSize(string url);

        Task DownloadTo(Stream stream, string fileUrl, IProgress<AudioDownloadProgress> progress = null);
    }
}