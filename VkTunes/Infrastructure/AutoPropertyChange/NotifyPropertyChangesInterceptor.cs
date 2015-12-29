using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;

using Caliburn.Micro;

using Ninject.Extensions.Interception;

namespace VkTunes.Infrastructure.AutoPropertyChange
{
    public class NotifyPropertyChangesInterceptor<T> : IInterceptor where T : class
    {
        private static ChangesTrackerCollection tracker = new ChangesTrackerCollection();

        private static HashSet<string> MethodsToSkip = new HashSet<string>(StringComparer.Ordinal)
        {
            nameof(PropertyChangedBase.NotifyOfPropertyChange)
        };

        public void Intercept(IInvocation invocation)
        {
            if (!MethodsToSkip.Contains(invocation.Request.Method.Name))
            {
                tracker.BeginTrack(invocation.Request.Target);
                try
                {
                    invocation.Proceed();
                }
                finally
                {
                    tracker.CompleteTrack(invocation.Request.Target, (sender, args) =>
                    {
                        var autoNotify = invocation.Request.Proxy as INotifyPropertyChangedEx;
                        autoNotify?.NotifyOfPropertyChange(args.PropertyName);
                    });
                }
            }
            else
                invocation.Proceed();
        }
    }

    public static class InterceptorFactory
    {
        public static IInterceptor CreateNotifyPropertyChangedInterceptor(Type type)
        {
            var interceptorType = typeof(NotifyPropertyChangesInterceptor<>);
            var generic = interceptorType.MakeGenericType(type);

            return (IInterceptor)Activator.CreateInstance(generic);
        }
    }
}