using Caliburn.Micro;

using VkTunes.Api.Models;

namespace VkTunes.SearchAudio
{
    public class SearchAudioViewModel : AudioListModelBase<SearchAudioCollection>
    {
        private string search;

        public SearchAudioViewModel(SearchAudioCollection audioCollection, IEventAggregator eventAggregator)
            : base(audioCollection, eventAggregator)
        {
        }

        public string Search
        {
            get { return search; }
            set
            {
                if (search != value)
                {
                    search = value;
                    AudioCollection.Query = value;
                    NotifyOfPropertyChange();
                }
            }
        }
    }
}