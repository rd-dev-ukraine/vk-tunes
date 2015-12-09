using Newtonsoft.Json;

namespace VkTunes.Api.Network
{
    public class VkApiResponse<TResponse>
        where TResponse : class
    {
        [JsonProperty("response")]
        public TResponse Response { get; set; }
    }
}