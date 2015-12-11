using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace VkTunes.Api.Queue
{
    /// <summary>
    /// Ensures requests are executed not frequently than 3 request per second.
    /// </summary>
    public class Throttler
    {
        private readonly int MaxRequestsPerSecond = 3;

        private readonly Queue<TimeRecord> taskStartTime = new Queue<TimeRecord>();

        public async Task Throttle(Func<Task> workload)
        {
            await Throttle(async () =>
            {
                await workload();
                return Task.FromResult(0);
            });
        }

        public async Task<TResult> Throttle<TResult>(Func<Task<TResult>> task)
        {
            if (task == null)
                throw new ArgumentNullException(nameof(task));

            var now = new TimeRecord { Ticks = DateTime.UtcNow.Ticks };
            TimeRecord lastTaskTime = null;

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

                    await Task.Delay(awaitMillisecond);
                }
            }

            now.Ticks = DateTime.UtcNow.Ticks;

            return await task();
        }

        private class TimeRecord
        {
            public long Ticks { get; set; }
        }
    }
}