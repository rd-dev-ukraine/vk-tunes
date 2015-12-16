using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace VkTunes.Api.Throttle
{
    /// <summary>
    /// Ensures requests are executed not frequently than 3 request per second.
    /// Throttler itself is passive and only thing it do it adds timeout before task if frequency exceeded.
    /// Task in throttler are executed in FIFO order.
    /// </summary>
    public class Throttler : IThrottler
    {
        private readonly DelayManager delayManager = new DelayManager();

        public async Task<TResult> Throttle<TResult>(Func<Task<TResult>> workload)
        {
            if (workload == null)
                throw new ArgumentNullException(nameof(workload));

            var delay = delayManager.RequestTimeout();
            Thread.Sleep(delay);
            delayManager.CompleteWork();

            return await workload();
        }

        public void Dispose()
        {
            delayManager.Dispose();
        }

        private class TimeRecord
        {
            public long Ticks { get; set; }
        }

        private class DelayManager : IDisposable
        {
            private readonly int MaxRequestsPerSecond = 3;

            private readonly Queue<TimeRecord> taskStartTime = new Queue<TimeRecord>();
            private readonly Mutex mutex = new Mutex();

            public int RequestTimeout()
            {
                mutex.WaitOne();
                TimeRecord lastTaskTime = null;

                var now = new TimeRecord { Ticks = DateTime.UtcNow.Ticks };

                taskStartTime.Enqueue(now);

                if (taskStartTime.Count > MaxRequestsPerSecond - 1)
                    lastTaskTime = taskStartTime.Dequeue();

                if (lastTaskTime != null)
                {
                    var elapsedSinceLastTime = TimeSpan.FromTicks(now.Ticks - lastTaskTime.Ticks);
                    if (elapsedSinceLastTime.TotalMilliseconds < 1000)
                    {
                        var awaitMillisecond = 1000 - (int)elapsedSinceLastTime.TotalMilliseconds;
                        Debug.WriteLine($"Throttler awaiting {awaitMillisecond}ms");
                        return awaitMillisecond;
                    }
                }

                return 0;
            }

            public void CompleteWork()
            {
                taskStartTime.Last().Ticks = DateTime.UtcNow.Ticks;
                mutex.ReleaseMutex();
            }

            public void Dispose()
            {
                mutex.Dispose();
            }
        }
    }
}