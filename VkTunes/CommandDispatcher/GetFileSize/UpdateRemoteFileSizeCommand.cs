namespace VkTunes.CommandDispatcher.GetFileSize
{
    public class UpdateRemoteFileSizeCommand : CommandBase
    {
        public UpdateRemoteFileSizeCommand(int audioId, int ownerId, string fileUrl)
        {
            AudioId = audioId;
            OwnerId = ownerId;
            FileUrl = fileUrl;
        }

        public int AudioId { get; }

        public int OwnerId { get; }

        public string FileUrl { get; }
    }
}