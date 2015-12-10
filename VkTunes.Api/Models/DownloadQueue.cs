using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

using VkTunes.Api.AudioStorage;
using VkTunes.Api.Client;

namespace VkTunes.Api.Models
{
    public class DownloadQueue
    {
        private readonly IVk vk;
        private readonly IVkAudioFileStorage storage;
        private readonly LinkedList<Download> queue = new LinkedList<Download>();
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

        public void AddToDownloadQueue(RemoteAudioRecord audio)
        {
            if (audio == null)
                throw new ArgumentNullException(nameof(audio));

            lock (syncRoot)
            {
                if (queue.Any(i => i.Audio.Id == audio.Id))
                    return;
            }

            var size = vk.FileSizePriore(audio.FileUrl).Result;

            var download = new Download(NotifyProgress)
            {
                Audio = audio,
                TotalBytes = (int)(size ?? 0)
            };

            lock (syncRoot)
                queue.AddLast(download);

            Run();
        }

        public DownloadProgress DownloadProgress()
        {
            var result = new DownloadProgress();

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
                        result.CurrentDownloadingAudio = item.Audio;
                        result.CurrentDownloadingTotalBytes = item.TotalBytes;
                        result.CurrentDownloadingCompletedBytes = item.BytesRead;
                    }
                }
            }

            return result;
        }

        private void Run()
        {
            lock (syncRoot)
                if (queue.Count > 0)
                    return;

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
                    var fileName = storage.GenerateFileName(current.Audio.Id, current.Audio.Artist, current.Audio.Title);
                    using (var destination = storage.OpenSave(fileName))
                    {
                        vk.DownloadTo(destination, current.Audio.FileUrl, current).Wait();
                        current.IsDownloadCompleted = true;
                        Progress?.Invoke(this, EventArgs.Empty);
                    }
                }
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

            public RemoteAudioRecord Audio { get; set; }

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