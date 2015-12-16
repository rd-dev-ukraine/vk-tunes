using System;

using Caliburn.Micro;

using VkTunes.Api.Models;

namespace VkTunes.DownloadProgress
{
    public class DownloadProgressViewModel : PropertyChangedBase, IHandle<EnqueueAudioDownloadEvent>
    {
        private readonly DownloadQueue queue;
        private DownloadProgressInfo info;

        public DownloadProgressViewModel(IEventAggregator eventAggregator, DownloadQueue queue)
        {
            if (queue == null)
                throw new ArgumentNullException(nameof(queue));

            eventAggregator.Subscribe(this);

            this.queue = queue;
            this.queue.Progress += (sender, args) =>
            {
                Execute.OnUIThread(() =>
                {
                    Info = this.queue.DownloadProgress();
                    IsDisplayed = Info.TotalAudioInQueue > 0;
                });
            };
        }

        public DownloadProgressInfo Info
        {
            get { return info; }
            set
            {
                info = value;
                IsDisplayed = info?.TotalAudioInQueue > 0;


                NotifyOfPropertyChange();
                NotifyOfPropertyChange(() => IsDisplayed);
            }
        }

        public bool IsDisplayed { get; private set; }

        public void Cancel()
        {
            queue.CancelDownloads();
        }

        public void Handle(EnqueueAudioDownloadEvent message)
        {
            if (message != null)
                queue.AddToDownloadQueue(message.AudioId, message.OwnerId);
        }
    }
}