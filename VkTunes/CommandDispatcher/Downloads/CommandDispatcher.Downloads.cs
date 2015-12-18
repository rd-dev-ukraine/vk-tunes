using System;
using System.Collections.Concurrent;
using System.IO;
using System.Linq;
using System.Threading;

using Caliburn.Micro;

using VkTunes.Api.Api;
using VkTunes.CommandDispatcher.AudioCommon;
using VkTunes.CommandDispatcher.Downloads;
using VkTunes.Utils;

// ReSharper disable once CheckNamespace
namespace VkTunes.CommandDispatcher
{
    public partial class CommandDispatcher : IHandle<DownloadAudioCommand>
    {
        private readonly ConcurrentQueue<AudioDownloadQueueElement> downloads = new ConcurrentQueue<AudioDownloadQueueElement>();
        private readonly ConcurrentQueue<AudioDownloadQueueElement> completedDownloads = new ConcurrentQueue<AudioDownloadQueueElement>();

        public void Handle(DownloadAudioCommand message)
        {
            // Don't add download if downloading enqueued
            if (downloads.Any(d => d.AudioId == message.AudioId && d.OwnerId == message.OwnerId))
                return;

            downloads.Enqueue(new AudioDownloadQueueElement(message.AudioId, message.OwnerId));

            if (downloads.Count == 1)
                ThreadPool.QueueUserWorkItem(_ => ProcessDownloadQueue());
        }

        private void ProcessDownloadQueue()
        {
            AudioDownloadQueueElement item;
            while (downloads.TryDequeue(out item))
            {
                item.IsStarted = true;
                NotifyDownloadStarted(item);

                var progress = new Progress(item, NotifyDownloadProgress);
                using (var buffer = new MemoryStream())
                {
                    var audio = vk.DownloadAudioFileTo(buffer, item.AudioId, item.OwnerId, progress).Result;
                    var localAudio = storage.Save(buffer, audio).Result;

                    item.IsCompleted = true;
                    completedDownloads.Enqueue(item);

                    NotifyDownloadCompleted(item);

                    Event(new LocalAudioUpdatedEvent(audio.Id, audio.Owner, localAudio));
                }
            }

            completedDownloads.Clear();

            Event(new DownloadProgressEvent(0, 0, DownloadProgressEvent.State.Completed, 0, 0, 0, 0));
        }

        private void NotifyDownloadStarted(AudioDownloadQueueElement item) => NotifyDownloadChanged(item, DownloadProgressEvent.State.Started);

        private void NotifyDownloadProgress(AudioDownloadQueueElement item) => NotifyDownloadChanged(item, DownloadProgressEvent.State.InProgress);

        private void NotifyDownloadCompleted(AudioDownloadQueueElement item) => NotifyDownloadChanged(item, DownloadProgressEvent.State.Completed);

        private void NotifyDownloadChanged(AudioDownloadQueueElement item, DownloadProgressEvent.State state)
        {
            var evt = new DownloadProgressEvent(
                item.AudioId,
                item.OwnerId,
                state,
                downloads.Count() + completedDownloads.Count(),
                completedDownloads.Count(),
                item.BytesCompleted,
                item.BytesTotal);

            Event(evt);
        }

        private class Progress : IProgress<AudioDownloadProgress>
        {
            private readonly AudioDownloadQueueElement item;
            private readonly Action<AudioDownloadQueueElement> notify;

            public Progress(
                AudioDownloadQueueElement item,
                Action<AudioDownloadQueueElement> notify)
            {
                if (item == null)
                    throw new ArgumentNullException(nameof(item));
                if (notify == null)
                    throw new ArgumentNullException(nameof(notify));

                this.item = item;
                this.notify = notify;
            }

            public void Report(AudioDownloadProgress value)
            {
                item.BytesCompleted = value.BytesRead;
                item.BytesTotal = value.TotalBytes;

                notify(item);
            }
        }
    }
}