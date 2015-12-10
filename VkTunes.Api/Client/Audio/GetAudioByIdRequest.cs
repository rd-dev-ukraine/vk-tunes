using Newtonsoft.Json;

using VkTunes.Api.Url;

namespace VkTunes.Api.Client.Audio
{
    public class GetAudioByIdRequest
    {
        [JsonProperty("audios")]
        [QueryStringName("audios")]
        public string AudioId { get; set; } 
    }
}