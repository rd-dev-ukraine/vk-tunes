using System;
using System.Linq;
using System.Threading.Tasks;

using Caliburn.Micro;

using VkTunes.AudioRecord;
using VkTunes.CommandDispatcher.SearchAudio;
using VkTunes.IoC;

// ReSharper disable DoNotCallOverridableMethodsInConstructor

namespace VkTunes.SearchAudio
{
    public class SearchAudioViewModel : Screen, IHandleWithTask<SearchAudioResultReceivedEvent>
    {
        private readonly IEventAggregator eventAggregator;
        private readonly IFactory<AudioRecordViewModel> audioRecordViewModelFactory; 
        private string search;

        public SearchAudioViewModel(
            IEventAggregator eventAggregator, 
            IFactory<AudioRecordViewModel> audioRecordViewModelFactory)
        {
            if (eventAggregator == null)
                throw new ArgumentNullException(nameof(eventAggregator));
            if (audioRecordViewModelFactory == null)
                throw new ArgumentNullException(nameof(audioRecordViewModelFactory));

            this.eventAggregator = eventAggregator;
            this.audioRecordViewModelFactory = audioRecordViewModelFactory;
            DisplayName = "Search audio";

            eventAggregator.Subscribe(this);
        }

        public BindableCollection<AudioRecordViewModel> Audio { get; } = new BindableCollection<AudioRecordViewModel>();

        public string Search
        {
            get { return search; }
            set
            {
                if (search != value)
                {
                    search = value;
                    NotifyOfPropertyChange();
                    eventAggregator.PublishOnBackgroundThread(new SearchAudioCommand(Search));
                }
            }
        }

        public async Task Handle(SearchAudioResultReceivedEvent message)
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
    }
}