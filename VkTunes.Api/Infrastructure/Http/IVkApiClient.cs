﻿using System.Threading.Tasks;

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
    }
}