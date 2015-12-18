namespace VkTunes.CommandDispatcher.AddRemoveAudio
{
    public class AddToMyAudioCommand : CommandBase
    {
        public AddToMyAudioCommand(int audioId, int ownerId)
        {
            AudioId = audioId;
            OwnerId = ownerId;
        }

        public int AudioId { get; }

        public int OwnerId { get; }
    }
}