using System;
using Caliburn.Micro;
using VkTunes.MyAudio;
using VkTunes.SearchAudio;

namespace VkTunes.AudioShell
{
    public class AudioShellViewModel : Conductor<IScreen>.Collection.OneActive
    {
        public AudioShellViewModel(
            MyAudioViewModel myAudio,
            SearchAudioViewModel searchAudio)
        {
            if (myAudio == null)
                throw new ArgumentNullException(nameof(myAudio));
            if (searchAudio == null)
                throw new ArgumentNullException(nameof(searchAudio));

            Items.Add(myAudio);
            Items.Add(searchAudio);

            // ReSharper disable once DoNotCallOverridableMethodsInConstructor
            ActivateItem(myAudio);
        }
    }
}