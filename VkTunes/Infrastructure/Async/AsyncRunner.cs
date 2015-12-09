using System;
using System.Threading.Tasks;

using Caliburn.Micro;

namespace VkTunes.Infrastructure.Async
{
    public class AsyncRunner : IAsync
    {
        private readonly IEventAggregator eventAggregator;

        public AsyncRunner(IEventAggregator eventAggregator)
        {
            if (eventAggregator == null)
                throw new ArgumentNullException(nameof(eventAggregator));

            this.eventAggregator = eventAggregator;
        }

        public void Execute<TResult>(Func<Task<TResult>> action, Action<TResult> processResult)
        {
            eventAggregator.PublishOnUIThread(new AsyncOperationStartedEvent());

            action().ContinueWith(task =>
            {
                Caliburn.Micro.Execute.OnUIThread(() => processResult(task.Result));
                eventAggregator.PublishOnUIThread(new AsyncOperationCompletedEvent());
            });
        }
    }
}