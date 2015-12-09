using Newtonsoft.Json;

namespace VkTunes.Api.Infrastructure.Http
{
    public class VkApiResponse<TResponse>
        where TResponse : class
    {
        [JsonProperty("response")]
        public TResponse Response { get; set; }
    }
}