using System;

using Caliburn.Micro;

namespace VkTunes.AudioRecord
{
    public class AudioRecordViewModel : PropertyChangedBase
    {
        private string title;
        private TimeSpan duration;
        private long? fileSize;

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
    }
}