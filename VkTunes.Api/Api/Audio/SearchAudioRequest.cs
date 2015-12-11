using VkTunes.Api.Url;

namespace VkTunes.Api.Api
{
    public class SearchAudioRequest
    {
        [QueryStringName("q")]
        public string Query { get; set; }
    }
}