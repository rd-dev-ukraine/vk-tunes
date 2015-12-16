using System;

namespace VkTunes.Api.Models.Collections
{
    public class AudioInfoUpdatedEventArgs : EventArgs
    {
        public AudioInfoUpdatedEventArgs(int audioId, AudioInfo audio)
        {
            if (audio == null)
                throw new ArgumentNullException(nameof(audio));
            AudioId = audioId;
            Audio = audio;
        }

        public int AudioId { get; }

        public AudioInfo Audio { get; }
    }
}