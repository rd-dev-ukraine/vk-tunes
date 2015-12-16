using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

using VkTunes.Api.LowLevel;
using VkTunes.Api.Throttle;

namespace VkTunes.Api.Api
{
    public class VkApi : IVkApi
    {
        private readonly IVkHttpClient client;
        private readonly IThrottler throttler;

        public VkApi(IVkHttpClient client, IThrottler throttler)
        {
            if (client == null)
                throw new ArgumentNullException(nameof(client));
            if (throttler == null)
                throw new ArgumentNullException(nameof(throttler));

            this.client = client;
            this.throttler = throttler;
        }

        public Task<UserAudioResponse> MyAudio()
        {
            return throttler.Throttle(() => client.CallApi<UserAudioResponse>("audio.get"));
        }

        public Task<SearchAudioResponse> SearchAudio(string q)
        {
            if (String.IsNullOrWhiteSpace(q))
                return Task.FromResult(new SearchAudioResponse
                {
                    Audio = new RemoteAudioRecord[] {},
                    Count = 0
                });

            return throttler.Throttle(() => client.CallApi<SearchAudioRequest, SearchAudioResponse>(
                                                            "audio.search",
                                                            new SearchAudioRequest
                                                            {
                                                                Query = q
                                                            }));
        }

        public async Task<RemoteAudioRecord> GetAudioById(int audioId, int ownerId)
        {
            var request = new GetAudioByIdRequest { AudioId = $"{ownerId}_{audioId}" };

            var all = await throttler.Throttle(() => client.CallApi<GetAudioByIdRequest, RemoteAudioRecord[]>("audio.getById", request));

            var audio = all.FirstOrDefault();
            if (audio == null)
                throw new VkApiCallException($"Call audio.getById(audioId:{audioId}, owner:{ownerId}) doesn't return an audio.");

            return audio;
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