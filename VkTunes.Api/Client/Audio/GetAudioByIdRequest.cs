using VkTunes.Api.Url;

namespace VkTunes.Api.Client.Audio
{
    public class GetAudioByIdRequest
    {
        [QueryStringName("audios")]
        public string AudioId { get; set; } 
    }
}