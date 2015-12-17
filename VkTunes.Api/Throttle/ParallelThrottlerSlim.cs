using System;
using System.Threading.Tasks;

using VkTunes.Api.Utils;

namespace VkTunes.Api.Throttle
{
    public class ParallelThrottlerSlim : IThrottler
    {
        private readonly TaskQueue queue;

        public ParallelThrottlerSlim()
        {
            queue = new TaskQueue(3);
        }

        public Task<TResult> Throttle<TResult>(Func<Task<TResult>> task)
        {
            var tcs = new TaskCompletionSource<TResult>();

            var unused = queue.Enqueue(() =>
            {
                tcs.Match(task());
                return Task.Delay(TimeSpan.FromSeconds(1.1));
            });

            return tcs.Task;
        }

        public void Dispose()
        {
            queue.Dispose();
        }
    }
}