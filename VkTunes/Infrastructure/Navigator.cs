using System;

using Caliburn.Micro;

namespace VkTunes.Infrastructure
{
    public class Navigator: INavigator
    {
        private readonly IEventAggregator eventAggregator;

        public Navigator(IEventAggregator eventAggregator)
        {
            if (eventAggregator == null)
                throw new ArgumentNullException(nameof(eventAggregator));

            this.eventAggregator = eventAggregator;
        }

        public void GoTo<TScreen>() where TScreen : IScreen
        {
            eventAggregator.PublishOnUIThread(new GoToViewModelEvent { ViewModel = typeof(TScreen) });
        }
    }
}