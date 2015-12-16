using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using VkTunes.Api.Queue;
using VkTunes.Api.Utils;

namespace VkTunes.Api.Test
{
    [TestClass]
    public class ThrottlerTest
    {
        [TestMethod]
        public async Task MultithreadThrottle()
        {
            var throttler = new AsyncThrottler();

            var random = new Random();
            var tasks = new List<Task>();
            for (var i = 0; i < 5; i++)
            {
                var index = i;
                
                ThreadPool.QueueUserWorkItem(_ =>
                {
                    for (var j = 0; j < 3; j++)
                    {
                        var taskId = j;

                        var taskIdentifier = $"Task {index}::{taskId} ";
                        var taskCompletionSource = new TaskCompletionSource<int>();
                        tasks.Add(taskCompletionSource.Task);
                        // Debug.WriteLine($"{taskIdentifier} scheduling at {DateTime.Now.TimeOfDay}");
                        throttler.Throttle(async () =>
                        {

                            var delay = random.Next(1, 700);
                            Debug.WriteLine($"{taskIdentifier} started at {DateTime.Now.TimeOfDay} will work for {delay}ms");
                            await Task.Delay(delay);

                            taskCompletionSource.SetResult(index);
                            // Debug.WriteLine($"{taskIdentifier} completed at {DateTime.Now.TimeOfDay}");
                        })
                        .FireAndForget();

                        // Debug.WriteLine($"{taskIdentifier} scheduled at {DateTime.Now.TimeOfDay}");
                    }

                    Thread.Sleep(random.Next(1, 600));
                });

                await Task.Delay(random.Next(0, 300));
            }

            await Task.WhenAll(tasks);
        }
    }
}