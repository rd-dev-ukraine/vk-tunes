using VkTunes.Api.AudioStorage;
using VkTunes.Api.Client;

namespace VkTunes.Api.Models
{
    public class AudioInfo
    {
        public int Id { get; set; } 

        public RemoteAudioRecord RemoteAudio { get; set; }

        public LocalAudioRecord LocalAudio { get; set; }

        public long? RemoteFileSize { get; set; }
    }
}