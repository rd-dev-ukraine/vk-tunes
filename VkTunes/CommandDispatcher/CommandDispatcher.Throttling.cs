using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace VkTunes.CommandDispatcher
{
    public partial class CommandDispatcher
    {
        private readonly ConcurrentDictionary<Type, Queue<IThrottlingRunner>> throttlingQueue = new ConcurrentDictionary<Type, Queue<IThrottlingRunner>>();

        private void ThrottleAll<TEvent>(TEvent @event, Func<TEvent, Task> handler, int throttlingIntervalMs = 300)
        {
            var queue = throttlingQueue.GetOrAdd(typeof(TEvent), type => new Queue<IThrottlingRunner>());

            lock (queue)
            {
                queue.Enqueue(new ThrottlingRunner<TEvent>(@event, handler, throttlingIntervalMs));

                if (queue.Count == 1)
                    ThreadPool.QueueUserWorkItem(RunThrottlingQueue, queue);
            }
        }

        private void Throttle<TEvent>(TEvent @event, Func<TEvent, Task> handler, int throttlingIntervalMs = 300)
        {
            var queue = throttlingQueue.GetOrAdd(typeof(TEvent), type => new Queue<IThrottlingRunner>());

            lock (queue)
            {
                var wasEmpty = queue.Count == 0;

                queue.Clear();
                queue.Enqueue(new ThrottlingRunner<TEvent>(@event, handler, throttlingIntervalMs));

                if (wasEmpty)
                    ThreadPool.QueueUserWorkItem(RunThrottlingQueue, queue);
            }
        }

        private void RunThrottlingQueue(object queueObj)
        {
            var queue = (Queue<IThrottlingRunner>)queueObj;

            while (true)
            {
                IThrottlingRunner task;
                lock (queue)
                {
                    if (queue.Count == 0)
                        return;

                    task = queue.Dequeue();
                }

                task.Run().Wait();
                Thread.Sleep(task.DelayMs);
            }
        }

        private interface IThrottlingRunner
        {
            Task Run();

            int DelayMs { get; }
        }

        private class ThrottlingRunner<TEvent> : IThrottlingRunner
        {
            private readonly TEvent @event;
            private readonly Func<TEvent, Task> handler;

            public ThrottlingRunner(TEvent @event, Func<TEvent, Task> handler, int delayMs)
            {
                if (@event == null)
                    throw new ArgumentNullException(nameof(@event));
                if (handler == null)
                    throw new ArgumentNullException(nameof(handler));

                this.@event = @event;
                this.handler = handler;
                DelayMs = delayMs;
            }

            public Task Run()
            {
                return handler(@event);
            }

            public int DelayMs { get; }
        }
    }
}