using System;

using VkTunes.Api.Api;

namespace VkTunes.CommandDispatcher.AddRemoveAudio
{
    public class MyAudioAddedEvent : EventBase
    {
        public MyAudioAddedEvent(RemoteAudioRecord audio)
        {
            if (audio == null)
                throw new ArgumentNullException(nameof(audio));
            Audio = audio;
        }

        public RemoteAudioRecord Audio { get; }
    }
}