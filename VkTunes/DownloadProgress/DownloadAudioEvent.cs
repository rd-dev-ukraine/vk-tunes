namespace VkTunes.DownloadProgress
{
    public class DownloadAudioEvent
    {
        public DownloadAudioEvent(int audioId, int ownerId)
        {
            AudioId = audioId;
            OwnerId = ownerId;
        }

        public int AudioId { get; set; }

        public int OwnerId { get; set; }
    }
}