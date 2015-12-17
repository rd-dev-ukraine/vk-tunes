using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace VkTunes.Api.Queue
{
    public class QueueItem<TResult> : IQueueItem
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