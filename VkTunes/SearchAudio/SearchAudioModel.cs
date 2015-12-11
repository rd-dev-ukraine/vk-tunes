using Caliburn.Micro;

using VkTunes.Api.Models;

namespace VkTunes.SearchAudio
{
    public class SearchAudioModel : AudioListModelBase
    {
        private string search;

        public SearchAudioModel(AudioCollectionBase audioCollection, IEventAggregator eventAggregator)
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
                    NotifyOfPropertyChange();
                }
            }
        }
    }
}