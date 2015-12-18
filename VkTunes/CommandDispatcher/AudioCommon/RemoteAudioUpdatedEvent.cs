using System;

using VkTunes.Api.Api;

namespace VkTunes.CommandDispatcher.AudioCommon
{
    public class RemoteAudioUpdatedEvent : EventBase
    {
        public RemoteAudioUpdatedEvent(RemoteAudioRecord audio)
        {
            if (audio == null)
                throw new ArgumentNullException(nameof(audio));
            Audio = audio;
        }

        public RemoteAudioRecord Audio { get; } 
    }
}