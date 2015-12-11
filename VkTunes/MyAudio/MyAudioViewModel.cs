using Caliburn.Micro;

using VkTunes.Api.Models;

namespace VkTunes.MyAudio
{
    public class MyAudioViewModel : AudioListModelBase<MyAudioCollection>
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