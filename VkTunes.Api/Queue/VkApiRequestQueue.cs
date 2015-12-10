using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace VkTunes.Api.Queue
{
    public class VkApiRequestQueue
    {
        private const int RequestIntervalMs = 300;

        private readonly LinkedList<IQueueItem> requestQueue = new LinkedList<IQueueItem>();
        private readonly object syncRoot = new object();

        private volatile bool isRunning;
        private long lastTicks;

        public Task<TResult> Enqueue<TResult>(Func<Task<TResult>> workload)
        {
            if (workload == null)
                throw new ArgumentNullException(nameof(workload));

            return EnqueueCore(workload, queueTask =>
            {
                lock (syncRoot)
                   requestQueue.AddLast(queueTask);
            });
        }

        public Task<TResult> EnqueuePriore<TResult>(Func<Task<TResult>> workload)
        {
            if (workload == null)
                throw new ArgumentNullException(nameof(workload));

            return EnqueueCore(workload, queueTask =>
            {
                lock (syncRoot)
                   requestQueue.AddFirst(queueTask);
            });
        }

        public void Clear()
        {
            lock (syncRoot)
            {
                requestQueue.Clear();
            }

            isRunning = false;
        }

        private Task<TResult> EnqueueCore<TResult>(Func<Task<TResult>> workload, Action<IQueueItem> addItemToQueue)
        {
            if (workload == null)
                throw new ArgumentNullException(nameof(workload));

            var queueTask = new TaskQueueItem<TResult>(workload);

            addItemToQueue(queueTask);
            Run();

            return queueTask.TaskCompletionSource.Task;
        }

        private void Run()
        {
            if (isRunning)
                return;

            ThreadPool.QueueUserWorkItem(s => ProcessTask());
        }

        private void ProcessTask()
        {
            isRunning = true;

            while (requestQueue.Count > 0)
            {
                Delay();

                IQueueItem item = null;

                lock (syncRoot)
                {
                    if (requestQueue.Count > 0)
                    {
                        item = requestQueue.First.Value;
                        requestQueue.RemoveFirst();
                    }
                }

                item?.Run();
            }

            isRunning = false;
        }

        private void Delay()
        {
            var ticks = DateTime.Now.Ticks;

            var elapsedSinceLastTask = lastTicks == 0 ? Int32.MaxValue : (int)TimeSpan.FromTicks(ticks - lastTicks).TotalMilliseconds;

            if (elapsedSinceLastTask < RequestIntervalMs)
                Thread.Sleep(RequestIntervalMs - elapsedSinceLastTask + 20);

            var nowTicks = DateTime.Now.Ticks;
            Interlocked.Exchange(ref lastTicks, nowTicks);
        }

        private interface IQueueItem
        {
            void Run();
        }

        private class TaskQueueItem<TResult> : IQueueItem
        {
            public TaskQueueItem(Func<Task<TResult>> workload)
            {
                if (workload == null)
                    throw new ArgumentNullException(nameof(workload));

                Workload = workload;
                TaskCompletionSource = new TaskCompletionSource<TResult>();
            }

            public Func<Task<TResult>> Workload { get; private set; }

            public TaskCompletionSource<TResult> TaskCompletionSource { get; private set; }

            public void Run()
            {
                var taskToRun = Workload();
                var result = taskToRun.Result;
                TaskCompletionSource.SetResult(result);
            }
        }
    }
}