using System;
using System.Collections.Concurrent;

namespace VkTunes.Utils
{
    public static class ConcurrentExtensions
    {
        public static void Clear<T>(this ConcurrentQueue<T> queue)
        {
            if (queue == null)
                throw new ArgumentNullException(nameof(queue));

            T _;
            while (queue.TryDequeue(out _))
            {
            }
        }
    }
}