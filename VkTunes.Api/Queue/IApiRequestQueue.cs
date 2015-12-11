using System;
using System.Threading.Tasks;

namespace VkTunes.Api.Queue
{
    public interface IApiRequestQueue
    {
        Throttler Throttler { get; }

        Task<TResult> EnqueueFirst<TResult>(Func<Task<TResult>> workload, int priority, string description);

        Task<TResult> EnqueueLast<TResult>(Func<Task<TResult>> workload, int priority, string description);

        void Clear(int priority);
    }
}