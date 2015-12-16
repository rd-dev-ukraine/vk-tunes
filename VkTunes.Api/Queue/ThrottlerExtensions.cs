using System;
using System.Threading.Tasks;

namespace VkTunes.Api.Queue
{
    public static class ThrottlerExtensions
    {
        public static async Task Throttle(this IThrottler throttler, Func<Task> workload)
        {
            await throttler.Throttle(async () =>
            {
                await workload();
                return Task.FromResult(0);
            });
        }
    }
}