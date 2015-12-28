namespace VkTunes.CommandDispatcher.AddRemoveAudio
{
    public class RemoveMyAudioCommand : CommandBase
    {
        public RemoveMyAudioCommand(int audioId, int ownerId)
        {
            AudioId = audioId;
            OwnerId = ownerId;
        }

        public int AudioId { get; }

        public int OwnerId { get; }
    }
}