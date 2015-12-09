using System;
using System.Net.Http;
using System.Threading.Tasks;

using Newtonsoft.Json;

using VkTunes.Api.Authorization;
using VkTunes.Api.Url;

namespace VkTunes.Api.Network
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
            {
                var httpResponse = await httpClient.PostAsync(new Uri(url), new StringContent(requestBody));
                if (!httpResponse.IsSuccessStatusCode)
                    throw new HttpErrorException($"HTTP API call error: {httpResponse.StatusCode} {httpResponse.ReasonPhrase}");

                var responseBody = await httpResponse.Content.ReadAsStringAsync();

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
    }
}