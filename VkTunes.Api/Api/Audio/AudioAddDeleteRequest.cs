using VkTunes.Api.Url;

namespace VkTunes.Api.Api
{
    public class AudioAddDeleteRequest
    {
        [QueryStringName("audio_id")]
        public int AudioId { get; set; }

        [QueryStringName("owner_id")]
        public int OwnerId { get; set; }
    }
}