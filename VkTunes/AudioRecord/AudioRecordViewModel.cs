using System;
using System.Threading.Tasks;

using Caliburn.Micro;

using VkTunes.Api.Authorization;
using VkTunes.Api.Models;
using VkTunes.CommandDispatcher.AudioCommon;

namespace VkTunes.AudioRecord
{
    public class AudioRecordViewModel : PropertyChangedBase, IHandleWithTask<AudioUpdatedEvent>
    {
        private readonly IEventAggregator eventAggregator;
        private readonly IAuthorizationInfo authorizationInfo;

        private string title;
        private string artist;
        private TimeSpan duration;
        private long? fileSize;
        private bool isInStorage;
        private bool isInVk;
        private string localFilePath;
        private bool isInMyAudio;

        public AudioRecordViewModel(
            IEventAggregator eventAggregator,
            IAuthorizationInfo authorizationInfo)
        {
            if (eventAggregator == null)
                throw new ArgumentNullException(nameof(eventAggregator));
            if (authorizationInfo == null)
                throw new ArgumentNullException(nameof(authorizationInfo));

            this.eventAggregator = eventAggregator;
            this.authorizationInfo = authorizationInfo;
            eventAggregator.Subscribe(this);
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
                NotifyOfPropertyChange();
                NotifyOfPropertyChange(() => FileSizeText);
                NotifyOfPropertyChange(() => IsFileSizeKnown);
                NotifyOfPropertyChange(() => Bitrate);
            }
        }

        public string FileSizeText => IsFileSizeKnown ? $"{FileSize / (1024M * 1024M):###.##}MB" : String.Empty;

        public bool IsFileSizeKnown => FileSize != null && FileSize != 0;

        public string Bitrate => IsFileSizeKnown ? $"{ 8 * (FileSize ?? 0) / 1024M / Math.Max(0.0000001M, (decimal)Duration.TotalSeconds): #####}Kbps" : String.Empty;

        public bool IsInStorage
        {
            get { return isInStorage; }
            set
            {
                isInStorage = value;
                NotifyOfPropertyChange();
            }
        }

        public string LocalFilePath
        {
            get { return localFilePath; }
            set
            {
                localFilePath = value;
                NotifyOfPropertyChange();
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

        //public void Download()
        //{
        //    eventAggregator.PublishOnBackgroundThread(new DownloadAudioEvent(Id, OwnerId));
        //}

        //public void AddToMyAudio()
        //{
        //    eventAggregator.PublishOnBackgroundThread(new AddAudioEvent(Id, OwnerId));
        //}

        //public void DeleteAudio()
        //{
        //    eventAggregator.PublishOnBackgroundThread(new DeleteAudioEvent(Id, OwnerId));
        //}

        public void Apply(AudioInfo audio)
        {
            Id = audio.Id;

            Duration = TimeSpan.FromSeconds(audio.RemoteAudio?.DurationInSeconds ?? 0);
            Artist = audio.RemoteAudio?.Artist;
            Title = audio.RemoteAudio?.Title;
            IsInStorage = audio.LocalAudio != null;
            IsInVk = audio.RemoteAudio != null;
            LocalFilePath = audio.LocalAudio?.FilePath;
            FileSize = audio.RemoteFileSize;
            OwnerId = audio.RemoteAudio?.Owner ?? 0;
            IsInMyAudio = audio.RemoteAudio?.Owner == authorizationInfo.UserId;
        }

        public Task Handle(AudioUpdatedEvent message)
        {
            return Execute.OnUIThreadAsync(() =>
            {
                if (message.Audio.Id == Id && message.Audio.RemoteAudio.Owner == OwnerId)
                    Apply(message.Audio);
            });
        }
    }
}