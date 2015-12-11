using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;

namespace VkTunes.Api.Queue
{
    /// <summary>
    /// Ensures requests are executed not frequently than 3 request per second.
    /// Throttler itself is passive and only thing it do it adds timeout before task if frequency exceeded.
    /// </summary>
    public class Throttler : IThrottler
    {
        private readonly object syncRoot = new object();
        private readonly int MaxRequestsPerSecond = 3;

        private readonly ConcurrentQueue<IQueueItem> workQueue = new ConcurrentQueue<IQueueItem>();

        private readonly Queue<TimeRecord> taskStartTime = new Queue<TimeRecord>();

        public async Task Throttle(Func<Task> workload)
        {
            await Throttle(async () =>
            {
                await workload();
                return Task.FromResult(0);
            });
        }

        public Task<TResult> Throttle<TResult>(Func<Task<TResult>> task)
        {
            if (task == null)
                throw new ArgumentNullException(nameof(task));

            var queueItem = new QueueItem<TResult>(task, -1, "Throttling");

            workQueue.Enqueue(queueItem);

            Run();

            return queueItem.ResultTask;
        }

        public async Task Run()
        {
            IQueueItem nextTask;

            while (workQueue.TryDequeue(out nextTask))
            {
                var now = new TimeRecord {Ticks = DateTime.UtcNow.Ticks};

                TimeRecord lastTaskTime = null;
                lock (syncRoot)
                {
                    taskStartTime.Enqueue(now);
                    if (taskStartTime.Count > MaxRequestsPerSecond - 1)
                        lastTaskTime = taskStartTime.Dequeue();
                }

                if (lastTaskTime != null)
                {
                    var elapsedSinceLastTime = TimeSpan.FromTicks(now.Ticks - lastTaskTime.Ticks);
                    if (elapsedSinceLastTime.TotalMilliseconds < 1000)
                    {
                        var awaitMillisecond = 1000 - (int) elapsedSinceLastTime.TotalMilliseconds;

                        Debug.WriteLine($"Throttler awaiting {awaitMillisecond}ms");

                        await Task.Delay(awaitMillisecond);
                    }
                }

                now.Ticks = DateTime.UtcNow.Ticks;

                await nextTask.Run();
            }
        }



        private class TimeRecord
        {
            public long Ticks { get; set; }
        }
    }
}