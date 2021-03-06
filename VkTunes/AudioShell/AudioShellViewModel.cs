﻿using System;
using Caliburn.Micro;

using VkTunes.DownloadProgress;
using VkTunes.MyAudio;
using VkTunes.SearchAudio;

namespace VkTunes.AudioShell
{
    public class AudioShellViewModel : Conductor<IScreen>.Collection.OneActive
    {
        public AudioShellViewModel(
            MyAudioViewModel myAudio,
            SearchAudioViewModel searchAudio,
            DownloadProgressViewModel downloadProgress)
        {
            if (myAudio == null)
                throw new ArgumentNullException(nameof(myAudio));
            if (searchAudio == null)
                throw new ArgumentNullException(nameof(searchAudio));
            if (downloadProgress == null)
                throw new ArgumentNullException(nameof(downloadProgress));

            Items.Add(myAudio);
            Items.Add(searchAudio);

            DownloadProgress = downloadProgress;
            NotifyOfPropertyChange(() => DownloadProgress);

            // ReSharper disable once DoNotCallOverridableMethodsInConstructor
            ActivateItem(myAudio);
        }

        public DownloadProgressViewModel DownloadProgress { get; }
    }
}