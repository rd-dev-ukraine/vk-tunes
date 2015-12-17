using System;

using Caliburn.Micro;

using VkTunes.CommandDispatcher.Downloads;

namespace VkTunes.DownloadProgress
{
    public class DownloadProgressViewModel : PropertyChangedBase, IHandle<DownloadProgressEvent>
    {
        private int downloadQueueLength;
        private int completedDownloads;
        private int currentDownloadBytes;
        private int currentDownloadSize;
        private bool isDisplayed;

        public DownloadProgressViewModel(IEventAggregator eventAggregator)
        {
            if (eventAggregator == null)
                throw new ArgumentNullException(nameof(eventAggregator));

            eventAggregator.Subscribe(this);
        }

        public int DownloadQueueLength
        {
            get { return downloadQueueLength; }
            set
            {
                downloadQueueLength = value;
                NotifyOfPropertyChange();
                NotifyOfPropertyChange(() => QueueProgress);
            }
        }

        public int CompletedDownloads
        {
            get { return completedDownloads; }
            set
            {
                completedDownloads = value;
                NotifyOfPropertyChange();
                NotifyOfPropertyChange(() => QueueProgress);
            }
        }

        public int CurrentDownloadBytes
        {
            get { return currentDownloadBytes; }
            set
            {
                currentDownloadBytes = value;
                NotifyOfPropertyChange();
                NotifyOfPropertyChange(() => CurrentDownloadProgress);
            }
        }

        public int CurrentDownloadSize
        {
            get { return currentDownloadSize; }
            set
            {
                currentDownloadSize = value;
                NotifyOfPropertyChange();
                NotifyOfPropertyChange(() => CurrentDownloadProgress);
            }
        }

        public bool IsDisplayed
        {
            get { return isDisplayed; }
            set
            {
                isDisplayed = value; 
                NotifyOfPropertyChange();
            }
        }

        public int QueueProgress => Progress(CompletedDownloads, DownloadQueueLength);

        public int CurrentDownloadProgress => Progress(CurrentDownloadBytes, CurrentDownloadSize);

        private int Progress(int currentValue, int totalValue)
        {
            if (totalValue == 0)
                return 0;

            var ratio = currentValue / (decimal)totalValue;
            return (int)(ratio * 100);
        }

        public void Handle(DownloadProgressEvent message)
        {
            IsDisplayed = message.QueueLength != 0;
            DownloadQueueLength = message.QueueLength;
            CompletedDownloads = message.CompletedDownloads;
            CurrentDownloadBytes = message.CompletedBytes;
            CurrentDownloadSize = message.AudioSize;
        }
    }
}