namespace VkTunes.CommandDispatcher.Downloads
{
    public class DownloadProgressEvent : EventBase
    {
        public DownloadProgressEvent(
            int audioId, 
            int ownerId, 
            State downloadingState, 
            int queueLength, 
            int completedDownloads, 
            int completedBytes, 
            int audioSize)
        {
            AudioId = audioId;
            OwnerId = ownerId;
            DownloadingState = downloadingState;
            QueueLength = queueLength;
            CompletedDownloads = completedDownloads;
            CompletedBytes = completedBytes;
            AudioSize = audioSize;
        }

        public int AudioId { get; }

        public int OwnerId { get; }

        public State DownloadingState { get; }

        public int QueueLength { get; }

        public int CompletedDownloads { get; }

        public int CompletedBytes { get; }

        public int AudioSize { get; }


        public enum State
        {
            Started,
            InProgress,
            Completed
        }
    }
}