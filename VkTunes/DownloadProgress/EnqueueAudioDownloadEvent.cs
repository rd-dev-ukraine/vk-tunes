namespace VkTunes.DownloadProgress
{
    public class EnqueueAudioDownloadEvent
    {
        public EnqueueAudioDownloadEvent(int audioId, int ownerId)
        {
            AudioId = audioId;
            OwnerId = ownerId;
        }

        public int AudioId { get; set; }

        public int OwnerId { get; set; }
    }
}