using Newtonsoft.Json;

namespace VkTunes.Api.LowLevel
{
    public class VkApiError
    {
        [JsonProperty("error")]
        public VkApiErrorDetails Error { get; set; }
    }
}