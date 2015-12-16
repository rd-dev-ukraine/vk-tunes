using System;

using Caliburn.Micro;

using VkTunes.DownloadProgress;

namespace VkTunes.AudioRecord
{
    public class AudioRecordViewModel : PropertyChangedBase
    {
        private readonly IEventAggregator eventAggregator;

        private string title;
        private string artist;
        private TimeSpan duration;
        private long? fileSize;
        private bool isInStorage;
        private bool isInVk;
        private string localFilePath;
        private bool isInMyAudio;

        public AudioRecordViewModel(IEventAggregator eventAggregator)
        {
            if (eventAggregator == null)
                throw new ArgumentNullException(nameof(eventAggregator));

            this.eventAggregator = eventAggregator;
        }

        public int Id { get; set; }

        public int OwnerId { get; set; }

        public string Artist
        {
            get { return artist; }
            set
            {
                artist = value;
                NotifyOfPropertyChange();
            }
        }

        public string Title
        {
            get { return title; }
            set
            {
                title = value;
                NotifyOfPropertyChange();
            }
        }

        public TimeSpan Duration
        {
            get { return duration; }
            set
            {
                duration = value;
                NotifyOfPropertyChange(() => Duration);
            }
        }

        public long? FileSize
        {
            get { return fileSize; }
            set
            {
                fileSize = value;
                NotifyOfPropertyChange(() => FileSize);
            }
        }

        public bool IsInStorage
        {
            get { return isInStorage; }
            set
            {
                isInStorage = value;
                NotifyOfPropertyChange(() => IsInStorage);
            }
        }

        public string LocalFilePath
        {
            get { return localFilePath; }
            set
            {
                localFilePath = value;
                NotifyOfPropertyChange(() => LocalFilePath);
            }
        }

        public bool IsInVk
        {
            get { return isInVk; }
            set
            {
                isInVk = value;
                NotifyOfPropertyChange(() => IsInVk);
            }
        }

        public bool IsInMyAudio
        {
            get { return isInMyAudio; }
            set
            {
                isInMyAudio = value;
                NotifyOfPropertyChange();
            }
        }

        public void Download()
        {
            eventAggregator.PublishOnBackgroundThread(new DownloadAudioEvent(Id, OwnerId));
        }

        public void AddToMyAudio()
        {
            eventAggregator.PublishOnBackgroundThread(new AddAudioEvent(Id, OwnerId));
        }

        public void DeleteAudio()
        {
            eventAggregator.PublishOnBackgroundThread(new DeleteAudioEvent(Id, OwnerId));
        }
    }
}