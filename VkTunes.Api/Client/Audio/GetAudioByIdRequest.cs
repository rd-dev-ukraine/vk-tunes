using VkTunes.Api.Url;

namespace VkTunes.Api
{
    public class GetAudioByIdRequest
    {
        [QueryStringName("audios")]
        public string AudioId { get; set; } 
    }
}