using System;

using VkTunes.Api.Models;

namespace VkTunes.CommandDispatcher.AudioCommon
{
    public class AudioUpdatedEvent : EventBase
    {
        public AudioUpdatedEvent(AudioInfo audio)
        {
            if (audio == null)
                throw new ArgumentNullException(nameof(audio));

            Audio = audio;
        }

        public AudioInfo Audio { get; }
    }
}