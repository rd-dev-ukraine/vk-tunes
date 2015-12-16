using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

using VkTunes.Api.Authorization;
using VkTunes.Api.LowLevel;
using VkTunes.Api.Throttle;

namespace VkTunes.Api.Api
{
    public class VkApi : IVkApi
    {
        private readonly IVkHttpClient client;
        private readonly IThrottler throttler;
        private readonly IAuthorizationInfo authorizationInfo;

        public VkApi(IVkHttpClient client, IThrottler throttler, IAuthorizationInfo authorizationInfo)
        {
            if (client == null)
                throw new ArgumentNullException(nameof(client));
            if (throttler == null)
                throw new ArgumentNullException(nameof(throttler));
            if (authorizationInfo == null)
                throw new ArgumentNullException(nameof(authorizationInfo));

            this.client = client;
            this.throttler = throttler;
            this.authorizationInfo = authorizationInfo;
        }

        public int MyUserId => authorizationInfo.UserId;

        public Task<UserAudioResponse> MyAudio()
        {
            return throttler.Throttle(() => client.CallApi<UserAudioResponse>("audio.get"));
        }

        public Task<SearchAudioResponse> SearchAudio(string q)
        {
            if (String.IsNullOrWhiteSpace(q))
                return Task.FromResult(new SearchAudioResponse
                {
                    Audio = new RemoteAudioRecord[] { },
                    Count = 0
                });

            return throttler.Throttle(() => client.CallApi<SearchAudioRequest, SearchAudioResponse>(
                                                            "audio.search",
                                                            new SearchAudioRequest
                                                            {
                                                                Query = q
                                                            }));
        }

        public async Task<RemoteAudioRecord> GetAudioByIdOrDefault(int audioId, int ownerId)
        {
            var request = new GetAudioByIdRequest { AudioId = $"{ownerId}_{audioId}" };

            var all = await throttler.Throttle(() => client.CallApi<GetAudioByIdRequest, RemoteAudioRecord[]>("audio.getById", request));

            return all.FirstOrDefault();
        }

        public async Task<RemoteAudioRecord> GetAudioById(int audioId, int ownerId)
        {
            var audio = await GetAudioByIdOrDefault(audioId, ownerId);
            if (audio == null)
                throw new VkApiCallException($"Call audio.getById(audioId:{audioId}, owner:{ownerId}) doesn't return an audio.");

            return audio;
        }

        public async Task<int> AddAudio(int audioId, int ownerId)
        {
            return await throttler.Throttle(() => client.CallApi<AudioAddDeleteRequest, int>(
                "audio.add",
                new AudioAddDeleteRequest
                {
                    AudioId = audioId,
                    OwnerId = ownerId
                }));
        }

        public async Task<int> DeleteAudio(int audioId, int ownerId)
        {
            return await throttler.Throttle(() => client.CallApi<AudioAddDeleteRequest, int>(
                "audio.delete",
                new AudioAddDeleteRequest
                {
                    AudioId = audioId,
                    OwnerId = ownerId
                }));
        }

        public Task<long?> GetFileSize(string url)
        {
            return throttler.Throttle(() => client.GetFileSize(url));
        }

        public async Task<RemoteAudioRecord> DownloadAudioFileTo(Stream stream, int audioId, int owner, IProgress<AudioDownloadProgress> progress)
        {
            if (stream == null)
                throw new ArgumentNullException(nameof(stream));

            var audio = await throttler.Throttle(() => GetAudioById(audioId, owner));
            await throttler.Throttle(() => client.DownloadTo(stream, audio.FileUrl, progress));
            return audio;
        }
    }
}