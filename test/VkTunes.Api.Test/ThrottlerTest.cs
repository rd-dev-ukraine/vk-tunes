using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using VkTunes.Api.Queue;

namespace VkTunes.Api.Test
{
    [TestClass]
    public class ThrottlerTest
    {
        [TestMethod]
        public async Task MultithreadThrottle()
        {
            var throttler = new Throttler();

            var random = new Random();
            var tasks = new List<Task>();
            for (var i = 0; i < 5; i++)
            {
                var index = i;
                var taskCompletionSource = new TaskCompletionSource<int>();
                tasks.Add(taskCompletionSource.Task);
                ThreadPool.QueueUserWorkItem(_ =>
                {
                    for (var j = 0; j < 3; j++)
                    {
                        throttler.Throttle(async () =>
                        {
                            var delay = random.Next(1, 700);
                            Debug.WriteLine($"Task {index} executed at {DateTime.Now.TimeOfDay} will work for {delay}ms");

                            await Task.Delay(delay);
                        })
                        .Wait();
                    }

                    Thread.Sleep(random.Next(1, 600));

                    taskCompletionSource.SetResult(index);
                });

                await Task.Delay(random.Next(0, 300));
            }

            await Task.WhenAll(tasks);
        }
    }
}