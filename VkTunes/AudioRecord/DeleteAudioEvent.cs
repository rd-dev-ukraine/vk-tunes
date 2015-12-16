namespace VkTunes.AudioRecord
{
    public class DeleteAudioEvent
    {
        public DeleteAudioEvent(int audioId, int ownerId)
        {
            AudioId = audioId;
            OwnerId = ownerId;
        }

        public int AudioId { get; } 

        public int OwnerId { get; }
    }
}