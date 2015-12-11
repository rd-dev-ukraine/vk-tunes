using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace VkTunes.Api.Queue
{
    /// <summary>
    /// Queue of tasks with priority. Queue executes max 3 requests per second.
    /// Each task in queue has a priority. Tasks with higher priority are executed first.
    /// </summary>
    public class PriorityApiRequestQueue : IApiRequestQueue
    {
        private const int ApiRequestPerSecond = 3;
        private readonly LinkedList<IQueueItem> taskQueue = new LinkedList<IQueueItem>();
        private readonly Queue<long> taskStartTime = new Queue<long>();
        private readonly Thread processLoop;
        private readonly object syncRoot = new object();

        public PriorityApiRequestQueue()
        {
            processLoop = new Thread(_ => ProcessQueue())
            {
                Name = "VK API Request Queue",
                IsBackground = true
            };
            processLoop.Start();
        }

        public Task<TResult> EnqueueFirst<TResult>(Func<Task<TResult>> workload, int priority, string description)
        {
            if (workload == null)
                throw new ArgumentNullException(nameof(workload));

            return EnqueueCore(workload, priority, description, InsertFirst);
        }

        public Task<TResult> EnqueueLast<TResult>(Func<Task<TResult>> workload, int priority, string description)
        {
            if (workload == null)
                throw new ArgumentNullException(nameof(workload));

            return EnqueueCore(workload, priority, description, InsertLast);
        }

        private Task<TResult> EnqueueCore<TResult>(Func<Task<TResult>> workload, int priority, string description, Action<IQueueItem> insertInQueue)
        {
            if (workload == null)
                throw new ArgumentNullException(nameof(workload));

            Debug.WriteLine($"Enqueue task with priority {priority} [{description}]");

            var item = new QueueItem<TResult>(workload, priority, description);

            insertInQueue(item);

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

        private void InsertFirst(IQueueItem item)
        {
            if (item == null)
                throw new ArgumentNullException(nameof(item));

            lock (syncRoot)
            {
                if (taskQueue.Count == 0)
                    taskQueue.AddFirst(item);
                else
                {
                    // Insert 2 in 3 3 2 2 1 1
                    //        2 in 3 3
                    //        2 in 1 1      
                    var node = taskQueue.First;

                    while (node != null)
                    {
                        if (node.Value.Priority <= item.Priority)
                        {
                            taskQueue.AddBefore(node, item);
                            return;
                        }

                        node = node.Previous;
                    }

                    taskQueue.AddLast(item);
                }
            }
        }


        private void InsertLast(IQueueItem item)
        {
            if (item == null)
                throw new ArgumentNullException(nameof(item));
            lock (syncRoot)
            {
                if (taskQueue.Count == 0)
                    taskQueue.AddFirst(item);
                else
                {
                    // Insert 2 in 3 3 2 2 1 1
                    //        2 in 3 3
                    //        2 in 1 1

                    var node = taskQueue.First;

                    while (node != null)
                    {
                        if (node.Value.Priority < item.Priority)
                        {
                            taskQueue.AddBefore(node, item);
                            return;
                        }

                        node = node.Previous;
                    }

                    taskQueue.AddLast(item);
                }
            }
        }

        private LinkedListNode<IQueueItem> LastOrDefault(Func<IQueueItem, bool> predicate)
        {
            var node = taskQueue.First;

            var isNodeFound = false;

            while (node != null)
            {
                var isNodeMatch = predicate(node.Value);

                if (isNodeMatch)
                {
                    isNodeFound = true;
                    node = node.Next;

                    continue;
                }

                if (isNodeFound)
                    return node;

                node = node.Next;
            }

            return taskQueue.Last;
        }

        private void ProcessQueue()
        {
            while (true)
            {
                IQueueItem nextTask;

                lock (syncRoot)
                {
                    var nextNode = taskQueue.First;
                    if (nextNode == null)
                        continue;

                    nextTask = nextNode.Value;
                    taskQueue.RemoveFirst();
                }

                taskStartTime.Enqueue(DateTime.Now.Ticks);
                if (taskStartTime.Count > ApiRequestPerSecond)
                    taskStartTime.Dequeue();

                if (taskStartTime.Count >= ApiRequestPerSecond)
                {
                    var lastTaskTicks = taskStartTime.First();
                    var elapsedSinceLastTime = TimeSpan.FromTicks(DateTime.Now.Ticks - lastTaskTicks);
                    if (elapsedSinceLastTime.TotalMilliseconds < 1000)
                        Thread.Sleep(1000 - (int)elapsedSinceLastTime.TotalMilliseconds);
                }

                nextTask.Run().Wait();
            }
        }

        private interface IQueueItem
        {
            int Priority { get; }

            Task Run();
        }

        private class QueueItem<TResult> : IQueueItem
        {
            public QueueItem(Func<Task<TResult>> workload, int priority, string description)
            {
                if (workload == null)
                    throw new ArgumentNullException(nameof(workload));

                Workload = workload;
                Priority = priority;
                CompletionSource = new TaskCompletionSource<TResult>();
                Description = description;
            }

            public int Priority { get; }

            public string Description { get; }

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

            public override string ToString()
            {
                return $"{Priority}:::{Description} ({ResultTask.Status})";
            }
        }
    }
}