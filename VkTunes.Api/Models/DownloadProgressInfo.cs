using VkTunes.Api.Client;
using VkTunes.Api.Client.Audio;

namespace VkTunes.Api.Models
{
    public class DownloadProgressInfo
    {
        public int TotalAudioInQueue { get; set; } 

        public int NumberOfAudioDownloadCompleted { get; set; }

        public int TotalQueueBytes { get; set; }

        public int QueueDownloadBytes { get; set; }

        public int CurrentDownloadingAudioId { get; set; }

        public int CurrentDownloadingTotalBytes { get; set; }

        public int CurrentDownloadingCompletedBytes { get; set; }
    }
}