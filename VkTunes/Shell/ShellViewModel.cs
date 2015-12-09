using System;

using Caliburn.Micro;

using Ninject;

using VkTunes.Infrastructure;

namespace VkTunes.Shell
{
    public class ShellViewModel : Conductor<IScreen>, IHandle<GoToViewModelEvent>
    {
        private readonly IKernel kernel;
        private readonly INavigator navigator;

        public ShellViewModel(
            IKernel kernel,
            IEventAggregator eventAggregator,
            INavigator navigator)
        {
            if (kernel == null)
                throw new ArgumentNullException(nameof(kernel));
            if (eventAggregator == null)
                throw new ArgumentNullException(nameof(eventAggregator));
            if (navigator == null)
                throw new ArgumentNullException(nameof(navigator));

            this.kernel = kernel;
            this.navigator = navigator;

            eventAggregator.Subscribe(this);
        }

        protected override void OnActivate()
        {
            base.OnActivate();
            //navigator.GoTo<ConnectToMetastormViewModel>();
            DisplayName = "VK-Tunes";
        }

        public void Handle(GoToViewModelEvent message)
        {
            var view = (IScreen)kernel.Get(message.ViewModel);
            ActivateItem(view);
        }
    }
}