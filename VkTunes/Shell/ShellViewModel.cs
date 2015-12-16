using System;
using Caliburn.Micro;
using Ninject;
using VkTunes.AudioShell;
using VkTunes.Authorization;
using VkTunes.DownloadProgress;
using VkTunes.Infrastructure.Navigation;

namespace VkTunes.Shell
{
    public class ShellViewModel : Conductor<IScreen>, IHandle<GoToViewModelEvent>
    {
        private readonly IKernel kernel;

        public ShellViewModel(
            IKernel kernel,
            IEventAggregator eventAggregator,
            AuthorizationViewModel authorizationViewModel,
            AudioShellViewModel audioShell,
            DownloadProgressViewModel downloadProgressViewModel)
        {
            if (kernel == null)
                throw new ArgumentNullException(nameof(kernel));
            this.kernel = kernel;
            if (eventAggregator == null)
                throw new ArgumentNullException(nameof(eventAggregator));
            if (authorizationViewModel == null)
                throw new ArgumentNullException(nameof(authorizationViewModel));
            if (audioShell == null)
                throw new ArgumentNullException(nameof(audioShell));
            if (downloadProgressViewModel == null)
                throw new ArgumentNullException(nameof(downloadProgressViewModel));

            eventAggregator.Subscribe(this);

            AuthorizationViewModel = authorizationViewModel;
            AudioShell = audioShell;
        }

        public AuthorizationViewModel AuthorizationViewModel { get; }

        public AudioShellViewModel AudioShell { get; }

        protected override void OnActivate()
        {
            base.OnActivate();
            DisplayName = "VK-Tunes";

            ActivateItem(AuthorizationViewModel);
        }

        public void Handle(GoToViewModelEvent message)
        {
            var viewInstance = (IScreen)kernel.Get(message.ViewModelType);
            ActivateItem(viewInstance);
        }
    }
}