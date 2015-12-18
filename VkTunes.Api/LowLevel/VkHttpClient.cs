using System;
using System.Diagnostics;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;

using Newtonsoft.Json;

using VkTunes.Api.Api;
using VkTunes.Api.Authorization;
using VkTunes.Api.Url;

namespace VkTunes.Api.LowLevel
{
    public class VkHttpClient : IVkHttpClient
    {
        private const string ApiUrl = "https://api.vk.com/method/";

        private readonly IAuthorizationInfo authorizationInfo;

        public VkHttpClient(IAuthorizationInfo authorizationInfo)
        {
            if (authorizationInfo == null)
                throw new ArgumentNullException(nameof(authorizationInfo));

            this.authorizationInfo = authorizationInfo;
        }

        public async Task<TResponse> CallApi<TRequest, TResponse>(string apiMethod, TRequest request)
        {
            if (String.IsNullOrWhiteSpace(apiMethod))
                throw new ArgumentNullException(nameof(apiMethod));
            if (request == null)
                throw new ArgumentNullException(nameof(request));

            if (String.IsNullOrWhiteSpace(authorizationInfo.Token))
                throw new AuthorizationRequiredException("Authorization required");

            var requestParam = new VkApiRequestParameters
            {
                AccessToken = authorizationInfo.Token,
                Version = "5.40"
            };

            var url = UrlBuilder.Build(ApiUrl + apiMethod, requestParam) + "&" + UrlBuilder.SerializeToQueryString(request);


            using (var httpClient = new HttpClient())
            using (var httpResponse = await httpClient.PostAsync(new Uri(url), new StringContent(String.Empty)))
            {
                if (!httpResponse.IsSuccessStatusCode)
                    throw new HttpErrorApiCallException("POST", url, $"HTTP API call error: {httpResponse.StatusCode} {httpResponse.ReasonPhrase}");

                var responseBody = await httpResponse.Content.ReadAsStringAsync();

                DebugOut(url, responseBody);

                var error = JsonConvert.DeserializeObject<VkApiError>(responseBody);
                if (error.Error != null)
                    throw new ErrorApiCallException(error.Error);

                var response = JsonConvert.DeserializeObject<VkApiResponse<TResponse>>(responseBody);
                return response.Response;
            }
        }

        public Task<TResponse> CallApi<TResponse>(string apiMethod)
        {
            return CallApi<string, TResponse>(apiMethod, String.Empty);
        }

        public async Task<long?> GetFileSize(string url)
        {
            using (var http = new HttpClient())
            using (var request = new HttpRequestMessage(HttpMethod.Head, url))
            {
                using (var response = await http.SendAsync(request))
                {
                    if (!response.IsSuccessStatusCode)
                        return null;

                    DebugOut($"Getting file size at {url}");

                    return response.Content.Headers.ContentLength;
                }
            }
        }

        public async Task DownloadTo(Stream stream, string fileUrl, IProgress<AudioDownloadProgress> progress = null)
        {
            if (stream == null)
                throw new ArgumentNullException(nameof(stream));
            if (String.IsNullOrWhiteSpace(fileUrl))
                throw new ArgumentNullException(nameof(fileUrl));

            using (var http = new HttpClient())
            using (var response = await http.GetAsync(fileUrl))
            {
                if (!response.IsSuccessStatusCode)
                    throw new HttpErrorApiCallException("GET", fileUrl, $"Downloading file error: {response.StatusCode} {response.ReasonPhrase}");

                var size = response.Content.Headers?.ContentLength ?? 0;
                using (var contentStream = await response.Content.ReadAsStreamAsync())
                {
                    var buffer = new byte[1024 * 4];
                    var totalBytesRead = 0;
                    while (true)
                    {
                        var bytesRead = await contentStream.ReadAsync(buffer, 0, buffer.Length);
                        await stream.WriteAsync(buffer, 0, bytesRead);

                        totalBytesRead += bytesRead;

                        progress?.Report(new AudioDownloadProgress
                        {
                            BytesRead = totalBytesRead,
                            TotalBytes = (int)size
                        });

                        if (bytesRead < buffer.Length)
                        {
                            if (size != 0 && totalBytesRead != size)
                                throw new InvalidOperationException("Non-robust reading");

                            break;
                        }
                    }
                }

                DebugOut($"Download {size} bytes from {fileUrl}");
            }
        }

        private void DebugOut(params string[] lines)
        {
            Debug.WriteLine("{0:HH:MM:ss.fff}", DateTime.Now);
            foreach (var l in lines)
                Debug.WriteLine(l);
            Debug.WriteLine("");
            Debug.WriteLine("");
        }
    }
}