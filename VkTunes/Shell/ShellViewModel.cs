﻿using System;

using Caliburn.Micro;

using Ninject;

using VkTunes.AudioList;
using VkTunes.Authorization;
using VkTunes.DownloadProgress;
using VkTunes.Infrastructure.Navigation;
using VkTunes.MyAudio;
using VkTunes.SearchAudio;

namespace VkTunes.Shell
{
    public class ShellViewModel : Conductor<IScreen>, IHandle<GoToViewModelEvent>
    {
        private readonly IKernel kernel;
        private readonly INavigator navigator;

        public ShellViewModel(
            IKernel kernel,
            IEventAggregator eventAggregator,
            INavigator navigator, 
            DownloadProgressViewModel downloadProgressViewModel)
        {
            if (kernel == null)
                throw new ArgumentNullException(nameof(kernel));
            if (eventAggregator == null)
                throw new ArgumentNullException(nameof(eventAggregator));
            if (navigator == null)
                throw new ArgumentNullException(nameof(navigator));

            this.kernel = kernel;
            this.navigator = navigator;
            this.Progress = downloadProgressViewModel;

            eventAggregator.Subscribe(this);
        }

        public DownloadProgressViewModel Progress { get; }

        public void MyAudio()
        {
            navigator.GoTo<MyAudioViewModel>();
        }

        public void SearchAudio()
        {
            navigator.GoTo<SearchAudioViewModel>();
        }

        protected override void OnActivate()
        {
            base.OnActivate();
            navigator.GoTo<AuthorizationViewModel>();
            DisplayName = "VK-Tunes";
        }

        public void Handle(GoToViewModelEvent message)
        {
            var view = (IScreen)kernel.Get(message.ViewModel);
            ActivateItem(view);
        }
    }
}