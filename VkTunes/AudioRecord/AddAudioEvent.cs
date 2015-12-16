namespace VkTunes.AudioRecord
{
    public class AddAudioEvent
    {
        public AddAudioEvent(int audioId, int ownerId)
        {
            AudioId = audioId;
            OwnerId = ownerId;
        }

        public int AudioId { get; } 

        public int OwnerId { get; }
    }
}