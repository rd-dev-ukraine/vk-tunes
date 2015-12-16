using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace VkTunes.Api.Throttle
{
    public class SlimThrottler : IThrottler
    {
        private readonly SemaphoreSlim semaphore = new SemaphoreSlim(1, 1);
        private readonly TaskDelaySource delaySource;

        public SlimThrottler()
        {
            delaySource = new TaskDelaySource(3, TimeSpan.FromSeconds(1));
        }

        public async Task<TResult> Throttle<TResult>(Func<Task<TResult>> task)
        {
            await semaphore.WaitAsync();
            try
            {
                await delaySource.Delay();
                return await task();
            }
            finally
            {
                semaphore.Release();
            }
        }

        public void Dispose()
        {
            semaphore.Dispose();
        }

        private class TaskDelaySource
        {
            private readonly int maxTasks;
            private readonly TimeSpan inInterval;
            private readonly Queue<long> ticks = new Queue<long>();

            public TaskDelaySource(int maxTasks, TimeSpan inInterval)
            {
                this.maxTasks = maxTasks;
                this.inInterval = inInterval;
            }

            public async Task Delay()
            {
                while (ticks.Count > maxTasks)
                    ticks.Dequeue();

                if (ticks.Any())
                {
                    var now = DateTime.UtcNow.Ticks;
                    var lastTick = ticks.First();
                    var intervalSinceLastTask = TimeSpan.FromTicks(now - lastTick);

                    if (intervalSinceLastTask < inInterval)
                        await Task.Delay((int)(inInterval - intervalSinceLastTask).TotalMilliseconds);
                }

                ticks.Enqueue(DateTime.UtcNow.Ticks);
            }
        }
    }
}