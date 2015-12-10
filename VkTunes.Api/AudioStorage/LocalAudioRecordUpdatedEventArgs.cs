using System;

namespace VkTunes.Api.AudioStorage
{
    public class LocalAudioRecordUpdatedEventArgs : EventArgs
    {
        public int AudioId { get; set; }

        public LocalAudioRecord LocalAudio { get; set; }
    }
}