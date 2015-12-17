namespace VkTunes.CommandDispatcher.Downloads
{
    public class DownloadAudioCommand : CommandBase
    {
        public DownloadAudioCommand(int audioId, int ownerId)
        {
            AudioId = audioId;
            OwnerId = ownerId;
        }

        public int AudioId { get; }

        public int OwnerId { get; }
    }
}