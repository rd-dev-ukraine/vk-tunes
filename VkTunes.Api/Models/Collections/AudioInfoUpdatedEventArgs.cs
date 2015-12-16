using System;

namespace VkTunes.Api.Models.Collections
{
    public class AudioInfoUpdatedEventArgs : EventArgs
    {
        public int AudioId { get; set; }

        public AudioInfo Audio { get; set; }
    }
}