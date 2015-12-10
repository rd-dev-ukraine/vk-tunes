using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;

using VkTunes.Api.AudioStorage;
using VkTunes.Api.Client;
using VkTunes.Api.Client.Audio;

namespace VkTunes.Api.Models
{
    public class DownloadQueue
    {
        private readonly IVk vk;
        private readonly IVkAudioFileStorage storage;
        private readonly LinkedList<Download> queue = new LinkedList<Download>();
        private readonly object syncRoot = new object();
        private bool isRunning;

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
                if (queue.Any(i => i.AudioId == audioId))
                    return;
            }

            var download = new Download(NotifyProgress)
            {
                AudioId = audioId,
                Owner = owner
            };

            lock (syncRoot)
                queue.AddLast(download);

            Run();
        }

        public DownloadProgressInfo DownloadProgress()
        {
            var result = new DownloadProgressInfo();

            lock (syncRoot)
            {
                foreach (var item in queue)
                {
                    result.TotalAudioInQueue++;

                    if (item.IsDownloadCompleted)
                        result.NumberOfAudioDownloadCompleted++;

                    result.TotalQueueBytes += item.TotalBytes;

                    if (item.IsDownloadStarted)
                        result.QueueDownloadBytes += item.BytesRead;

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

        private void Run()
        {
            if (isRunning)
                return;

            isRunning = true;

            ThreadPool.QueueUserWorkItem(_ =>
            {
                while (true)
                {
                    Download current = null;

                    lock (syncRoot)
                        current = queue.FirstOrDefault(e => !e.IsDownloadStarted);

                    if (current == null)
                    {
                        lock (syncRoot)
                        {
                            queue.Clear();
                            Progress?.Invoke(this, EventArgs.Empty);
                            break;
                        }
                    }

                    current.IsDownloadStarted = true;
                    using (var buffer = new MemoryStream())
                    {
                        var audio = vk.DownloadTo(buffer, current.AudioId, current.Owner, current).Result;
                        storage.Save(buffer, audio);

                        current.IsDownloadCompleted = true;
                        Progress?.Invoke(this, EventArgs.Empty);
                    }
                }

                isRunning = false;
            });
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