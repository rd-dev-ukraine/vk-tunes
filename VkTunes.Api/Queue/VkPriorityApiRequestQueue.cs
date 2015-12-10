using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace VkTunes.Api.Queue
{
    /// <summary>
    /// Queue of tasks with priority. Queue executes max 3 requests per second.
    /// Each task in queue has a priority. Tasks with higher priority are executed first.
    /// </summary>
    public class VkPriorityApiRequestQueue : IApiRequestQueue
    {
        private const int ApiRequestPerSecond = 3;
        private readonly LinkedList<IQueueItem> taskQueue = new LinkedList<IQueueItem>();
        private readonly Queue<long> taskStartTime = new Queue<long>();
        private readonly object syncRoot = new object();
        private bool isRunning;

        public Task<TResult> EnqueueFirst<TResult>(Func<Task<TResult>> workload, int priority)
        {
            if (workload == null)
                throw new ArgumentNullException(nameof(workload));

            return EnqueueCore(workload, priority, q => q.Priority > priority);
        }

        public Task<TResult> EnqueueLast<TResult>(Func<Task<TResult>> workload, int priority)
        {
            if (workload == null)
                throw new ArgumentNullException(nameof(workload));

            return EnqueueCore(workload, priority, q => q.Priority >= priority);
        }

        private Task<TResult> EnqueueCore<TResult>(Func<Task<TResult>> workload, int priority, Func<IQueueItem, bool> findPreviousNode)
        {
            if (workload == null)
                throw new ArgumentNullException(nameof(workload));

            var item = new QueueItem<TResult>(workload, priority);

            lock (syncRoot)
            {
                var prev = LastOrDefault(findPreviousNode);
                if (prev == null)
                    taskQueue.AddFirst(item);
                else
                    taskQueue.AddAfter(prev, item);
            }

            Run();

            return item.ResultTask;
        }

        public void Clear(int priority)
        {
            lock (syncRoot)
            {
                var node = taskQueue.First;
                while (node != null)
                {
                    var next = node.Next;

                    if (node.Value.Priority == priority)
                        taskQueue.Remove(node);

                    node = next;
                }
            }
        }

        private async Task Run()
        {
            if (isRunning)
                return;

            isRunning = true;

            IQueueItem nextTask = null;

            lock (syncRoot)
            {
                var nextNode = taskQueue.First;
                if (nextNode == null)
                {
                    isRunning = false;
                    return;
                }

                nextTask = nextNode.Value;
                taskQueue.RemoveFirst();
            }

            taskStartTime.Enqueue(DateTime.Now.Ticks);
            if (taskStartTime.Count > ApiRequestPerSecond)
                taskStartTime.Dequeue();

            await nextTask.Run();

            if (taskStartTime.Count >= ApiRequestPerSecond)
            {
                var lastTaskTicks = taskStartTime.Last();
                var elapsedSinceLastTime = TimeSpan.FromTicks(DateTime.Now.Ticks - lastTaskTicks);
                if (elapsedSinceLastTime.TotalMilliseconds < 1000)
                    await Task.Delay(1000 - (int)elapsedSinceLastTime.TotalMilliseconds);
            }

            Run();
        }

        private LinkedListNode<IQueueItem> LastOrDefault(Func<IQueueItem, bool> predicate)
        {
            var node = taskQueue.First;

            while (node != null)
            {
                if (predicate(node.Value))
                    return node;

                node = node.Next;
            }

            return null;
        }

        private interface IQueueItem
        {
            int Priority { get; }

            Task Run();
        }

        private class QueueItem<TResult> : IQueueItem
        {
            public QueueItem(Func<Task<TResult>> workload, int priority)
            {
                if (workload == null)
                    throw new ArgumentNullException(nameof(workload));

                Workload = workload;
                Priority = priority;
                CompletionSource = new TaskCompletionSource<TResult>();
            }

            public int Priority { get; }

            public Task<TResult> ResultTask => CompletionSource.Task;

            private TaskCompletionSource<TResult> CompletionSource { get; }

            private Func<Task<TResult>> Workload { get; }

            public async Task Run()
            {
                var taskToRun = Workload();

                try
                {
                    var result = await taskToRun;
                    CompletionSource.SetResult(result);
                }
                catch (Exception ex)
                {
                    CompletionSource.SetException(ex);
                }
            }
        }
    }
}