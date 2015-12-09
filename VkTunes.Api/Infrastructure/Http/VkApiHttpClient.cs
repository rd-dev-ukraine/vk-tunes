using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

using Newtonsoft.Json;

using VkTunes.Api.Authorization;
using VkTunes.Api.Url;

namespace VkTunes.Api.Infrastructure.Http
{
    public class VkApiHttpClient : IVkApiClient
    {
        private const string ApiUrl = "https://api.vk.com/method/";

        private readonly IAuthorizationInfo authorizationInfo;

        public VkApiHttpClient(IAuthorizationInfo authorizationInfo)
        {
            if (authorizationInfo == null)
                throw new ArgumentNullException(nameof(authorizationInfo));

            this.authorizationInfo = authorizationInfo;
        }

        public async Task<TResponse> CallApi<TRequest, TResponse>(string apiMethod, TRequest request)
            where TRequest : class
            where TResponse : class
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

            var url = UrlBuilder.Build(ApiUrl + apiMethod, requestParam);
            var requestBody = JsonConvert.SerializeObject(request);

            using (var httpClient = new HttpClient())
            using (var httpResponse = await httpClient.PostAsync(new Uri(url), new StringContent(requestBody)))
            {
                if (!httpResponse.IsSuccessStatusCode)
                    throw new HttpErrorException($"HTTP API call error: {httpResponse.StatusCode} {httpResponse.ReasonPhrase}");

                var responseBody = await httpResponse.Content.ReadAsStringAsync();

                Debug.WriteLine(">>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>");
                Debug.WriteLine("{0:HH:MM:ss.fff}", DateTime.Now);
                Debug.WriteLine(url);
                Debug.WriteLine(requestBody);
                Debug.WriteLine(responseBody);
                Debug.WriteLine("<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<");

                var error = JsonConvert.DeserializeObject<VkApiError>(responseBody);
                if (error.Error != null)
                    throw new VkApiErrorResponseException(error.Error);

                var response = JsonConvert.DeserializeObject<VkApiResponse<TResponse>>(responseBody);
                return response.Response;
            }
        }

        public Task<TResponse> CallApi<TResponse>(string apiMethod) where TResponse : class
        {
            return CallApi<string, TResponse>(apiMethod, String.Empty);
        }

        public async Task<long?> FileSize(string url)
        {
            using (var http = new HttpClient())
            using (var request = new HttpRequestMessage(HttpMethod.Head, url))
            {
                request.Headers.UserAgent.ParseAdd("Mozilla/5.0 (Windows NT 10.0; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/47.0.2526.73 Safari/537.36");
                request.Headers.Accept.ParseAdd("*/*");
                request.Headers.Add("X-Requested-With", "XMLHttpRequest");

                using (var response = await http.SendAsync(request))
                {
                    if (!response.IsSuccessStatusCode)
                        return null;

                    Debug.WriteLine(">>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>");
                    Debug.WriteLine("{0:HH:MM:ss.fff}", DateTime.Now);
                    Debug.WriteLine(url);
                    Debug.WriteLine("<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<");

                    return response.Content.Headers.ContentLength;
                }
            }
        }
    }
}