using System;
using System.Collections.Generic;
using System.Diagnostics;
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
        private readonly LinkedList<IQueueItem> taskQueue = new LinkedList<IQueueItem>();
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

                nextTask.Run().Wait();
            }
        }
    }
}