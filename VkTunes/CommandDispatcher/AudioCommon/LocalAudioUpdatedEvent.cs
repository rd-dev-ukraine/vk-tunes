using System;

using VkTunes.Api.AudioStorage;

namespace VkTunes.CommandDispatcher.AudioCommon
{
    public class LocalAudioUpdatedEvent : EventBase
    {
        public LocalAudioUpdatedEvent(int audioId, int ownerId, LocalAudioRecord audio)
        {
            if (audio == null)
                throw new ArgumentNullException(nameof(audio));

            AudioId = audioId;
            OwnerId = ownerId;
            Audio = audio;
        }

        public int AudioId { get; }

        public int OwnerId { get; }

        public LocalAudioRecord Audio { get; }
    }
}