using System;

using VkTunes.Api.Models;

namespace VkTunes.CommandDispatcher.SearchAudio
{
    public class SearchAudioResultReceivedEvent : EventBase
    {
        public SearchAudioResultReceivedEvent(AudioInfo[] audio, string query)
        {
            if (audio == null)
                throw new ArgumentNullException(nameof(audio));

            Audio = audio;
            Query = query;
        }

        public string Query { get; }

        public AudioInfo[] Audio { get; }
    }
}