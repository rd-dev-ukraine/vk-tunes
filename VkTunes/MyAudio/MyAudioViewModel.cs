using System;
using System.Linq;
using System.Threading.Tasks;

using Caliburn.Micro;

using VkTunes.AudioRecord;
using VkTunes.CommandDispatcher.Downloads;
using VkTunes.CommandDispatcher.MyAudio;

// ReSharper disable DoNotCallOverridableMethodsInConstructor

namespace VkTunes.MyAudio
{
    public class MyAudioViewModel : Screen, IHandleWithTask<MyAudioLoadedEvent>
    {
        private readonly IEventAggregator eventAggregator;

        public MyAudioViewModel(IEventAggregator eventAggregator)
        {
            if (eventAggregator == null)
                throw new ArgumentNullException(nameof(eventAggregator));

            this.eventAggregator = eventAggregator;
            DisplayName = "My audio";

            eventAggregator.Subscribe(this);
        }

        public BindableCollection<AudioRecordViewModel> Audio { get; } = new BindableCollection<AudioRecordViewModel>();

        protected override void OnActivate()
        {
            base.OnActivate();

            eventAggregator.PublishOnBackgroundThread(new MyAudioLoadCommand());
        }

        public async Task Handle(MyAudioLoadedEvent message)
        {
            await Execute.OnUIThreadAsync(() =>
            {
                Audio.Clear();
                Audio.AddRange(message.Audio.Select(a => new AudioRecordViewModel(eventAggregator, a)));
            });
        }

        public void DownloadAll()
        {
            foreach (var audio in Audio.Where(a => !a.IsInStorage))
                eventAggregator.PublishOnBackgroundThread(new DownloadAudioCommand(audio.Id, audio.OwnerId));
        }
    }
}