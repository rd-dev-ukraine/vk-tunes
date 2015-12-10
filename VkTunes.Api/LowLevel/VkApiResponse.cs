using Newtonsoft.Json;

namespace VkTunes.Api.LowLevel
{
    public class VkApiResponse<TResponse>
        where TResponse : class
    {
        [JsonProperty("response")]
        public TResponse Response { get; set; }
    }
}