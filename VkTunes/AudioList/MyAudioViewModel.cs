using Caliburn.Micro;

using VkTunes.Api.Models;

namespace VkTunes.AudioList
{
    public class MyAudioViewModel : AudioListModelBase
    {
        public MyAudioViewModel(MyAudioCollection myAudio, IEventAggregator eventAggregator) 
            : base(myAudio, eventAggregator)
        {
        }

        protected override void OnActivate()
        {
            base.OnActivate();
            Reload();
        }
    }
}