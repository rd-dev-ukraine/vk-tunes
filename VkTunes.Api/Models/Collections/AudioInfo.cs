using VkTunes.Api.Api;
using VkTunes.Api.AudioStorage;

namespace VkTunes.Api.Models.Collections
{
    public class AudioInfo
    {
        public int Id { get; set; } 

        public RemoteAudioRecord RemoteAudio { get; set; }

        public LocalAudioRecord LocalAudio { get; set; }

        public long? RemoteFileSize { get; set; }
    }
}