namespace VkTunes.CommandDispatcher.GetFileSize
{
    public class RemoteFileSizeUpdatedEvent : EventBase
    {
        public RemoteFileSizeUpdatedEvent(int audioId, int ownerId, long fileSize)
        {
            AudioId = audioId;
            OwnerId = ownerId;
            FileSize = fileSize;
        }

        public int AudioId { get; }

        public int OwnerId { get; }

        public long FileSize { get; }
    }
}