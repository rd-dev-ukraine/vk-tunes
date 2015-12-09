using System;

using Caliburn.Micro;

namespace VkTunes.AudioRecord
{
    public class AudioRecordViewModel : PropertyChangedBase
    {
        private string title;
        private TimeSpan duration;
        private long? fileSize;
        private bool isInStorage;
        private bool isInVk;
        private string localFilePath;

        public int Id { get; set; }

        public string Title
        {
            get { return title; }
            set
            {
                title = value;
                NotifyOfPropertyChange(() => Title);
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
    }
}