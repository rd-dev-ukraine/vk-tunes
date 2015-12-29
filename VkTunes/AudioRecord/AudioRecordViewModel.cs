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
    [AutoNotifyOnPropertyChange]
    public class AudioRecordViewModel : PropertyChangedBase,
        IHandle<LocalAudioUpdatedEvent>,
        IHandle<RemoteAudioUpdatedEvent>,
        IHandle<RemoteFileSizeUpdatedEvent>
    {
        private readonly IEventAggregator eventAggregator;
        private readonly IAuthorizationInfo authorizationInfo;

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
        }

        public virtual int Id { get; set; }

        public virtual int OwnerId { get; set; }

        public virtual string Artist { get; set; }

        public virtual string Title { get; set; }

        public virtual TimeSpan Duration { get; set; }

        public virtual long? FileSize { get; set; }

        public virtual string FileSizeText => IsFileSizeKnown ? $"{FileSize / (1024M * 1024M):###.##}MB" : String.Empty;

        public virtual bool IsFileSizeKnown => FileSize != null && FileSize != 0;

        public virtual string Bitrate => IsFileSizeKnown ? $"{ 8 * (FileSize ?? 0) / 1024M / Math.Max(0.0000001M, (decimal)Duration.TotalSeconds): #####}Kbps" : String.Empty;

        public virtual bool IsInStorage => !String.IsNullOrWhiteSpace(LocalFilePath);

        public virtual string LocalFilePath { get; set; }

        public virtual bool IsInMyAudio { get; set; }

        public virtual bool CanAddToMyAudio => !IsInMyAudio;

        public virtual bool CanRemoveFromMyAudio => IsInMyAudio;

        public virtual void Download()
        {
            eventAggregator.PublishOnBackgroundThread(new DownloadAudioCommand(Id, OwnerId));
        }

        public virtual void AddToMyAudio()
        {
            eventAggregator.PublishOnBackgroundThread(new AddToMyAudioCommand(Id, OwnerId));
        }

        public virtual void RemoveFromMyAudio()
        {
            eventAggregator.PublishOnBackgroundThread(new RemoveMyAudioCommand(Id, OwnerId));
        }

        public virtual void Apply(AudioInfo audio)
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

            LocalFilePath = audio.FilePath;
        }

        public virtual void Handle(RemoteAudioUpdatedEvent message)
        {
            if (message.Audio.Id == Id && message.Audio.Owner == OwnerId)
                Execute.OnUIThread(() => ApplyRemoteAudio(message.Audio));
        }

        public virtual void Handle(LocalAudioUpdatedEvent message)
        {
            if (message.AudioId == Id && message.OwnerId == OwnerId)
                Execute.OnUIThread(() => ApplyLocalAudio(message.Audio));
        }

        public virtual void Handle(RemoteFileSizeUpdatedEvent message)
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