using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;

using Caliburn.Micro;

using NProxy.Core;

namespace VkTunes.Infrastructure.AutoPropertyChange
{
    public class NotifyPropertyChangesInvokationHandler : IInvocationHandler
    {
        private static readonly HashSet<string> MethodsToSkip = new HashSet<string>(StringComparer.Ordinal)
        {
            nameof(PropertyChangedBase.NotifyOfPropertyChange),
            nameof(Object.GetHashCode),
            nameof(Object.Equals),
        };
        private static readonly ChangesTrackerCollection Tracker = new ChangesTrackerCollection();

        public object Invoke(object target, MethodInfo methodInfo, object[] parameters)
        {
            Debug.WriteLine($"Invokation of {methodInfo.Name}");

            if (MethodsToSkip.Contains(methodInfo.Name))
                return methodInfo.Invoke(target, parameters);

            try
            {
                Tracker.BeginTrack(target);
                return methodInfo.Invoke(target, parameters);
            }
            finally
            {
                Tracker.CompleteTrack(target, (sender, args) =>
                {
                    (target as INotifyPropertyChangedEx)?.NotifyOfPropertyChange(args.PropertyName);
                });
            }
        }
    }

    public class NotifyPropertyChangedInterceptionFilter : IInterceptionFilter
    {
        public bool AcceptEvent(EventInfo eventInfo)
        {
            return true;
        }

        public bool AcceptProperty(PropertyInfo propertyInfo)
        {
            return true;
        }

        public bool AcceptMethod(MethodInfo methodInfo)
        {
            return methodInfo.DeclaringType != typeof(object) && methodInfo.DeclaringType != typeof(PropertyChangedBase);
        }
    }
}