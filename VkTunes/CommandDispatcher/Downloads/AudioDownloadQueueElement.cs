namespace VkTunes.CommandDispatcher.Downloads
{
    public class AudioDownloadQueueElement
    {
        public AudioDownloadQueueElement(int audioId, int ownerId)
        {
            AudioId = audioId;
            OwnerId = ownerId;
        }

        public int AudioId { get; }

        public int OwnerId { get; }

        public bool IsStarted { get; set; }

        public bool IsCompleted { get; set; }

        public int BytesCompleted { get; set; }

        public int BytesTotal { get; set; }
    }
}