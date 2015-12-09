using System;
using System.Threading.Tasks;

namespace VkTunes.Infrastructure.Async
{
    public interface IAsync
    {
        void Execute<TResult>(Func<Task<TResult>> action, Action<TResult> processResult);
    }
}