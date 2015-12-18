using System;
using System.Linq;
using System.Threading.Tasks;

using Caliburn.Micro;

using VkTunes.AudioRecord;
using VkTunes.CommandDispatcher.SearchAudio;

// ReSharper disable DoNotCallOverridableMethodsInConstructor

namespace VkTunes.SearchAudio
{
    public class SearchAudioViewModel : Screen, IHandleWithTask<SearchAudioResultReceivedEvent>
    {
        private readonly IEventAggregator eventAggregator;
        private string search;

        public SearchAudioViewModel(IEventAggregator eventAggregator)
        {
            if (eventAggregator == null)
                throw new ArgumentNullException(nameof(eventAggregator));

            this.eventAggregator = eventAggregator;
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
                Audio.AddRange(message.Audio.Select(a => new AudioRecordViewModel(eventAggregator, a)));
            });
        }
    }
}