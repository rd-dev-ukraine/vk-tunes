using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace VkTunes.Api.Queue
{
    public class AsyncThrottler : IThrottler
    {
        // ReSharper disable once PrivateFieldCanBeConvertedToLocalVariable
        private readonly Thread throttlerThread;

        private readonly AutoResetEvent evt = new AutoResetEvent(false);
        private readonly ConcurrentQueue<IRequest> requestQueue = new ConcurrentQueue<IRequest>();
        private readonly Queue<long> requestTime = new Queue<long>();

        public AsyncThrottler()
        {
            throttlerThread = new Thread(ProcessQueue)
            {
                Name = "VK Throttling Queue",
                IsBackground = true
            };
            throttlerThread.Start();
        }

        public Task<TResult> Throttle<TResult>(Func<Task<TResult>> task)
        {
            var request = new TaskRequest<TResult>(task);

            requestQueue.Enqueue(request);
            evt.Set();

            return request.ResultTask;
        }

        private void ProcessQueue(object state)
        {
            while (true)
            {
                evt.WaitOne();

                IRequest request;
                while (requestQueue.TryDequeue(out request))
                {
                    Delay();

                    request.Run();
                }

            }
            // ReSharper disable once FunctionNeverReturns
        }

        private void Delay()
        {
            while (requestTime.Count > 3)
                requestTime.Dequeue();

            // Get last request time
            if (requestTime.Any())
            {

                var last = requestTime.First();
                var now = DateTime.UtcNow.Ticks;

                var timeSinceLastRequest = TimeSpan.FromTicks(now - last);

                if (timeSinceLastRequest.TotalMilliseconds < 1000)
                {
                    var delay = 1000 - timeSinceLastRequest.TotalMilliseconds;
                    Thread.Sleep((int) delay);
                }
            }

            requestTime.Enqueue(DateTime.UtcNow.Ticks);
        }

        public void Dispose()
        {
        }

        private interface IRequest
        {
            void Run();
        }

        private class TaskRequest<TResult> : IRequest
        {
            private readonly Func<Task<TResult>> workload;
            private readonly TaskCompletionSource<TResult> completionSource;

            public TaskRequest(Func<Task<TResult>> workload)
            {
                if (workload == null)
                    throw new ArgumentNullException(nameof(workload));

                this.workload = workload;
                completionSource = new TaskCompletionSource<TResult>();
            }

            public Task<TResult> ResultTask => completionSource.Task;

            public void Run()
            {
                var task = workload();
                task.ContinueWith(r =>
                {
                    try
                    {
                        completionSource.SetResult(r.Result);
                    }
                    catch (Exception ex)
                    {
                        completionSource.SetException(ex);
                    }
                });
            }
        }
    }
}