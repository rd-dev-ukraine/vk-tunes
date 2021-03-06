﻿using System;
using System.IO;
using System.Threading.Tasks;

namespace VkTunes.Api.Api
{
    public interface IVkApi
    {
        Task<UserAudioResponse> MyAudio();

        Task<RemoteAudioRecord> GetAudioById(int audioId, int ownerId);

        Task<int> AddAudio(int audioId, int ownerId);

        Task<long?> GetFileSize(string url);

        Task<RemoteAudioRecord> DownloadAudioFileTo(Stream stream, int audioId, int owner, IProgress<AudioDownloadProgress> progress);

        Task<SearchAudioResponse> SearchAudio(string q);

        Task<int> DeleteAudio(int audioId, int ownerId);

        Task<RemoteAudioRecord> GetAudioByIdOrDefault(int audioId, int ownerId);

        int MyUserId { get; }
    }
}