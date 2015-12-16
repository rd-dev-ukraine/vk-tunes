using Caliburn.Micro;

using VkTunes.Api.Models;
using VkTunes.Api.Models.Collections;

// ReSharper disable DoNotCallOverridableMethodsInConstructor

namespace VkTunes.MyAudio
{
    public class MyAudioViewModel : AudioListModelBase<MyAudioCollection>
    {
        public MyAudioViewModel(MyAudioCollection myAudio, IEventAggregator eventAggregator) 
            : base(myAudio, eventAggregator)
        {
            DisplayName = "My audio";
        }

        protected override void OnActivate()
        {
            base.OnActivate();
            Reload();
        }
    }
}