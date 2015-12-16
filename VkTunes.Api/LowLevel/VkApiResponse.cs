using Newtonsoft.Json;

namespace VkTunes.Api.LowLevel
{
    public class VkApiResponse<TResponse>
    {
        [JsonProperty("response")]
        public TResponse Response { get; set; }
    }
}