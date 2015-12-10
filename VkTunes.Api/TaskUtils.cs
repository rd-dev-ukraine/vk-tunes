﻿using System;
using System.Threading.Tasks;

namespace VkTunes.Api
{
    public static class TaskUtils
    {
        public static Task<Tuple<T1, T2>> WhenAll<T1, T2>(Task<T1> first, Task<T2> second)
        {
            var t1 = first.ContinueWith(r => (object)r);
            var t2 = second.ContinueWith(r => (object)r);

            return Task.WhenAll(t1, t2).ContinueWith(r =>
            {
                var f = (Task<T1>)r.Result[0];
                var s = (Task<T2>)r.Result[1];

                return new Tuple<T1, T2>(f.Result, s.Result);
            });
        }
    }
}