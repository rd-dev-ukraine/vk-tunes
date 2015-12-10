using VkTunes.Api.Models;

namespace VkTunes.AudioList
{
    public class MyAudioViewModel : AudioListModelBase
    {
        public MyAudioViewModel(MyAudioCollection myAudio) 
            : base(myAudio)
        {
        }

        protected override void OnActivate()
        {
            base.OnActivate();
            Reload();
        }
    }
}