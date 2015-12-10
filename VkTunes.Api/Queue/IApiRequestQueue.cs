using System;
using System.Threading.Tasks;

namespace VkTunes.Api.Queue
{
    public interface IApiRequestQueue
    {
        Task<TResult> EnqueueFirst<TResult>(Func<Task<TResult>> workload, int priority);
        Task<TResult> EnqueueLast<TResult>(Func<Task<TResult>> workload, int priority);
        void Clear(int priority);
    }
}