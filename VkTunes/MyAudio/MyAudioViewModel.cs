using System;
using System.Linq;
using System.Threading.Tasks;

using Caliburn.Micro;

using VkTunes.Api.Models;
using VkTunes.AudioRecord;
using VkTunes.CommandDispatcher.AddRemoveAudio;
using VkTunes.CommandDispatcher.Downloads;
using VkTunes.CommandDispatcher.MyAudio;
using VkTunes.IoC;

// ReSharper disable DoNotCallOverridableMethodsInConstructor

namespace VkTunes.MyAudio
{
    public class MyAudioViewModel : Screen, IHandleWithTask<MyAudioLoadedEvent>, IHandle<MyAudioAddedEvent>, IHandle<MyAudioRemovedEvent>
    {
        private readonly IEventAggregator eventAggregator;
        private readonly IFactory<AudioRecordViewModel> audioRecordViewModelFactory;

        public MyAudioViewModel(
            IEventAggregator eventAggregator,
            IFactory<AudioRecordViewModel> audioRecordViewModelFactory)
        {
            if (eventAggregator == null)
                throw new ArgumentNullException(nameof(eventAggregator));
            if (audioRecordViewModelFactory == null)
                throw new ArgumentNullException(nameof(audioRecordViewModelFactory));

            this.eventAggregator = eventAggregator;
            this.audioRecordViewModelFactory = audioRecordViewModelFactory;
            DisplayName = "My audio";

            eventAggregator.Subscribe(this);
        }

        public BindableCollection<AudioRecordViewModel> Audio { get; } = new BindableCollection<AudioRecordViewModel>();

        protected override void OnActivate()
        {
            base.OnActivate();

            eventAggregator.PublishOnBackgroundThread(new MyAudioLoadCommand());
        }

        public void DownloadAll()
        {
            foreach (var audio in Audio.Where(a => !a.IsInStorage))
                eventAggregator.PublishOnBackgroundThread(new DownloadAudioCommand(audio.Id, audio.OwnerId));
        }

        public async Task Handle(MyAudioLoadedEvent message)
        {
            await Execute.OnUIThreadAsync(() =>
            {
                Audio.Clear();
                Audio.AddRange(message.Audio.Select(a =>
                {
                    var model = audioRecordViewModelFactory.CreateInstance();
                    model.Apply(a);
                    return model;
                }));
            });
        }

        public void Handle(MyAudioAddedEvent message)
        {
            if (message.Audio != null)
            {
                var model = audioRecordViewModelFactory.CreateInstance();
                model.Apply(new AudioInfo
                {
                    Id = message.Audio.Id,
                    RemoteAudio = message.Audio
                });

                Audio.Insert(0, model);
            }
        }

        public void Handle(MyAudioRemovedEvent message)
        {
            var audio = Audio.FirstOrDefault(a => a.Id == message.AudioId && a.OwnerId == message.OwnerId);
            if (audio != null)
                Audio.Remove(audio);
        }
    }
}