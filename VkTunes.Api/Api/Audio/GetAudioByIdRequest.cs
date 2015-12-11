using VkTunes.Api.Url;

namespace VkTunes.Api.Api
{
    public class GetAudioByIdRequest
    {
        [QueryStringName("audios")]
        public string AudioId { get; set; } 
    }
}