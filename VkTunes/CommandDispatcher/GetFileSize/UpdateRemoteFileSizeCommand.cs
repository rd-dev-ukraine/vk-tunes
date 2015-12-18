namespace VkTunes.CommandDispatcher.GetFileSize
{
    public class UpdateRemoteFileSizeCommand : CommandBase
    {
        public UpdateRemoteFileSizeCommand(int audioId, int ownerId, string fileUrl, bool asap = false)
        {
            AudioId = audioId;
            OwnerId = ownerId;
            FileUrl = fileUrl;
            Asap = asap;
        }

        public int AudioId { get; }

        public int OwnerId { get; }

        public string FileUrl { get; }

        public bool Asap { get; }
    }
}