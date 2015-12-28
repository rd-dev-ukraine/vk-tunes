namespace VkTunes.CommandDispatcher.AddRemoveAudio
{
    public class MyAudioRemovedEvent : EventBase
    {
        public MyAudioRemovedEvent(int audioId, int ownerId)
        {
            AudioId = audioId;
            OwnerId = ownerId;
        }

        public int AudioId { get; }

        public int OwnerId { get; }
    }
}