using System;
using System.Threading.Tasks;

namespace VkTunes.Api.Queue
{
    public interface IThrottler : IDisposable
    {
        Task Throttle(Func<Task> workload);

        Task<TResult> Throttle<TResult>(Func<Task<TResult>> task);
    }
}