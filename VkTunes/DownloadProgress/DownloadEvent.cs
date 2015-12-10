using VkTunes.Api.Client;

namespace VkTunes.DownloadProgress
{
    public class DownloadEvent
    {
        public int AudioId { get; set; }

        public int OwnerId { get; set; }
    }
}