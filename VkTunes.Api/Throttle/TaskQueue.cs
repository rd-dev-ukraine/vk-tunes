﻿using System;
using System.Threading;
using System.Threading.Tasks;

namespace VkTunes.Api.Throttle
{
    public class TaskQueue : IDisposable
    {
        private readonly SemaphoreSlim semaphore;

        public TaskQueue(int concurrentRequests)
        {
            semaphore = new SemaphoreSlim(concurrentRequests);
        }

        public async Task<T> Enqueue<T>(Func<Task<T>> taskGenerator)
        {
            await semaphore.WaitAsync();
            try
            {
                return await taskGenerator();
            }
            finally
            {
                semaphore.Release();
            }
        }


        public async Task Enqueue(Func<Task> taskGenerator)
        {
            await semaphore.WaitAsync();
            try
            {
                await taskGenerator();
            }
            finally
            {
                semaphore.Release();
            }
        }

        public void Dispose()
        {
            semaphore.Dispose();
        }
    }
}