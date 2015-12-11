using Newtonsoft.Json;

namespace VkTunes.Api.Api
{
    public class UserAudioResponse
    {
        [JsonProperty("count")]
        public int Count { get; set; } 

        [JsonProperty("items")]
        public RemoteAudioRecord[] Audio { get; set; }
    }
}