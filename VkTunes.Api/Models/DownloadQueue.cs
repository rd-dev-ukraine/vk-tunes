using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using VkTunes.Api.AudioStorage;
using VkTunes.Api.Queue;

namespace VkTunes.Api.Models
{
    public class DownloadQueue
    {
        private readonly IVk vk;
        private readonly IVkAudioFileStorage storage;
        private readonly HashSet<Download> downloads = new HashSet<Download>();
        private readonly object syncRoot = new object();

        public DownloadQueue(IVk vk, IVkAudioFileStorage storage)
        {
            if (vk == null)
                throw new ArgumentNullException(nameof(vk));
            if (storage == null)
                throw new ArgumentNullException(nameof(storage));

            this.vk = vk;
            this.storage = storage;
        }

        public void AddToDownloadQueue(int audioId, int owner)
        {
            lock (syncRoot)
            {
                if (downloads.Any(d => d.AudioId == audioId))
                    return;
            }

            var download = new Download(NotifyProgress)
            {
                AudioId = audioId,
                Owner = owner
            };

            lock (syncRoot)
            {
                downloads.Add(download);
            }

            EnqueueDownload(download);
        }

        private async Task EnqueueDownload(Download download)
        {
            download.IsDownloadStarted = true;
            using (var buffer = new MemoryStream())
            {
                var audio = await vk.DownloadAudioFileTo(buffer, download.AudioId, download.Owner, download);
                await storage.Save(buffer, audio);

                download.IsDownloadCompleted = true;
                Progress?.Invoke(this, EventArgs.Empty);
            }

            lock (downloads)
                if (downloads.All(d => d.IsDownloadCompleted))
                    downloads.Clear();
        }

        public DownloadProgressInfo DownloadProgress()
        {
            var result = new DownloadProgressInfo();

            lock (syncRoot)
            {
                foreach (var item in downloads)
                {
                    result.TotalAudioInQueue++;

                    if (item.IsDownloadCompleted)
                        result.NumberOfAudioDownloadCompleted++;

                    if (item.IsDownloadStarted && !item.IsDownloadCompleted)
                    {
                        result.CurrentDownloadingAudioId = item.AudioId;
                        result.CurrentDownloadingTotalBytes = item.TotalBytes;
                        result.CurrentDownloadingCompletedBytes = item.BytesRead;
                    }
                }
            }

            return result;
        }

        private void NotifyProgress(Download download)
        {
            Progress?.Invoke(this, EventArgs.Empty);
        }

        public event EventHandler<EventArgs> Progress;

        private class Download : IProgress<AudioDownloadProgress>
        {
            private readonly Action<Download> notifyProgress;

            public Download(Action<Download> notifyProgress)
            {
                if (notifyProgress == null)
                    throw new ArgumentNullException(nameof(notifyProgress));

                this.notifyProgress = notifyProgress;
            }

            public int AudioId { get; set; }

            public int Owner { get; set; }

            public int TotalBytes { get; set; }

            public int BytesRead { get; set; }

            public bool IsDownloadStarted { get; set; }

            public bool IsDownloadCompleted { get; set; }

            public void Report(AudioDownloadProgress value)
            {
                if (value == null)
                    throw new ArgumentNullException(nameof(value));

                TotalBytes = value.TotalBytes;
                BytesRead = value.BytesRead;

                notifyProgress(this);
            }
        }
    }
}