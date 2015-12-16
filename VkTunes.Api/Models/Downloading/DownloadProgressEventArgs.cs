using System;

namespace VkTunes.Api.Models.Downloading
{
    public class DownloadProgressEventArgs : EventArgs
    {
        public DownloadProgressEventArgs(DownloadProgressInfo progress)
        {
            if (progress == null)
                throw new ArgumentNullException(nameof(progress));

            Progress = progress;
        }

        public DownloadProgressInfo Progress { get; }
    }
}