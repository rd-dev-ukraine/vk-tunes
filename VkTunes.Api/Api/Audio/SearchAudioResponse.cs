using Newtonsoft.Json;

namespace VkTunes.Api.Api
{
    public class SearchAudioResponse
    {
        [JsonProperty("count")]
        public int Count { get; set; } 

        [JsonProperty("items")]
        public RemoteAudioRecord[] Audio { get; set; }
    }
}