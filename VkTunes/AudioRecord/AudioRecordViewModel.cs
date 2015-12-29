using System;

using Caliburn.Micro;

using VkTunes.Api.Api;
using VkTunes.Api.AudioStorage;
using VkTunes.Api.Authorization;
using VkTunes.Api.Models;
using VkTunes.CommandDispatcher.AddRemoveAudio;
using VkTunes.CommandDispatcher.AudioCommon;
using VkTunes.CommandDispatcher.Downloads;
using VkTunes.CommandDispatcher.GetFileSize;
using VkTunes.Infrastructure.AutoPropertyChange;

namespace VkTunes.AudioRecord
{
    public class AudioRecordViewModel : PropertyChangedBase,
        IHandle<LocalAudioUpdatedEvent>,
        IHandle<RemoteAudioUpdatedEvent>,
        IHandle<RemoteFileSizeUpdatedEvent>,
        IRaiseNotifyPropertyChanged
    {
        private readonly IEventAggregator eventAggregator;
        private readonly IAuthorizationInfo authorizationInfo;

        private string title;
        private string artist;
        private TimeSpan duration;
        private long? fileSize;
        private bool isInStorage;
        private string localFilePath;
        private bool isInMyAudio;

        protected AudioRecordViewModel()
        {
        }

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

        public bool IsInMyAudio
        {
            get { return isInMyAudio; }
            set
            {
                isInMyAudio = value;
                NotifyOfPropertyChange();
                NotifyOfPropertyChange(() => CanAddToMyAudio);
                NotifyOfPropertyChange(() => CanRemoveFromMyAudio);
            }
        }

        public bool CanAddToMyAudio => !IsInMyAudio;

        public bool CanRemoveFromMyAudio => IsInMyAudio;

        public void Download()
        {
            eventAggregator.PublishOnBackgroundThread(new DownloadAudioCommand(Id, OwnerId));
        }

        public void AddToMyAudio()
        {
            eventAggregator.PublishOnBackgroundThread(new AddToMyAudioCommand(Id, OwnerId));
        }

        public void RemoveFromMyAudio()
        {
            eventAggregator.PublishOnBackgroundThread(new RemoveMyAudioCommand(Id, OwnerId));
        }

        

        //public void DeleteAudio()
        //{
        //    eventAggregator.PublishOnBackgroundThread(new DeleteAudioEvent(Id, OwnerId));
        //}

        public void Apply(AudioInfo audio)
        {
            Id = audio.Id;

            if (audio.RemoteAudio != null)
                ApplyRemoteAudio(audio.RemoteAudio);
            if (audio.LocalAudio != null)
                ApplyLocalAudio(audio.LocalAudio);
        }

        private void ApplyRemoteAudio(RemoteAudioRecord audio)
        {
            if (audio == null)
                throw new ArgumentNullException(nameof(audio));

            Duration = TimeSpan.FromSeconds(audio.DurationInSeconds);
            Artist = audio.Artist;
            Title = audio.Title;

            OwnerId = audio.Owner;
            IsInMyAudio = audio.Owner == authorizationInfo.UserId;
        }

        private void ApplyLocalAudio(LocalAudioRecord audio)
        {
            if (audio == null)
                throw new ArgumentNullException(nameof(audio));

            IsInStorage = audio != null;
            LocalFilePath = audio.FilePath;
        }

        public void Handle(RemoteAudioUpdatedEvent message)
        {
            if (message.Audio.Id == Id && message.Audio.Owner == OwnerId)
                Execute.OnUIThread(() => ApplyRemoteAudio(message.Audio));
        }

        public void Handle(LocalAudioUpdatedEvent message)
        {
            if (message.AudioId == Id && message.OwnerId == OwnerId)
                Execute.OnUIThread(() => ApplyLocalAudio(message.Audio));
        }

        public void Handle(RemoteFileSizeUpdatedEvent message)
        {
            if (message.AudioId == Id && message.OwnerId == OwnerId)
                Execute.OnUIThread(() => FileSize = message.FileSize);
        }

        public void RaiseNotifyPropertyChanged(string propertyName)
        {
            NotifyOfPropertyChange(propertyName);
        }
    }
}