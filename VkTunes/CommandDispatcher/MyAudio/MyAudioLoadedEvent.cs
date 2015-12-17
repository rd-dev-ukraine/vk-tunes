using System;

using VkTunes.Api.Models;

namespace VkTunes.CommandDispatcher.MyAudio
{
    public class MyAudioLoadedEvent : EventBase
    {
        public MyAudioLoadedEvent(AudioInfo[] audio)
        {
            if (audio == null)
                throw new ArgumentNullException(nameof(audio));

            Audio = audio;
        }

        public AudioInfo[] Audio { get; }
    }
}