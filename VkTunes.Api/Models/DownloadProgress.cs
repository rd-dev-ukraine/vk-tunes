using VkTunes.Api.Client;

namespace VkTunes.Api.Models
{
    public class DownloadProgress
    {
        public int TotalAudioInQueue { get; set; } 

        public int NumberOfAudioDownloadCompleted { get; set; }

        public int TotalQueueBytes { get; set; }

        public int QueueDownloadBytes { get; set; }

        public RemoteAudioRecord CurrentDownloadingAudio { get; set; }

        public int CurrentDownloadingTotalBytes { get; set; }

        public int CurrentDownloadingCompletedBytes { get; set; }
    }
}