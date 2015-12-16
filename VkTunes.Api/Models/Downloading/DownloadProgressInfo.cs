namespace VkTunes.Api.Models.Downloading
{
    public class DownloadProgressInfo
    {
        public int TotalAudioInQueue { get; set; }

        public int NumberOfAudioDownloadCompleted { get; set; }

        public int CurrentDownloadingAudioId { get; set; }

        public int CurrentDownloadingTotalBytes { get; set; }

        public int CurrentDownloadingCompletedBytes { get; set; }

        public int TotalDownloadProgress => TotalAudioInQueue == 0 ? 0 : (int)(NumberOfAudioDownloadCompleted/ ((decimal)TotalAudioInQueue) * 100);

        public int CurrentDownloadProgress => CurrentDownloadingTotalBytes == 0 ? 0 : (int)(CurrentDownloadingCompletedBytes / ((decimal)CurrentDownloadingTotalBytes) * 100);

        public string DownloadingCountStatus => $"Downloaded {NumberOfAudioDownloadCompleted} from {TotalAudioInQueue} audio";

        public string CurrentDownloadingBytesStatus => $"Downloaded {CurrentDownloadingCompletedBytes / 1024:0.##} from {CurrentDownloadingTotalBytes / 1024:0.##} bytes";
    }
}