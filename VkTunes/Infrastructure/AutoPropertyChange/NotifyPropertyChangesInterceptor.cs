using System;

using Ninject.Extensions.Interception;

namespace VkTunes.Infrastructure.AutoPropertyChange
{
    public class NotifyPropertyChangesInterceptor<T> : IInterceptor where T : class
    {
        public void Intercept(IInvocation invocation)
        {
            var checker = new DirtyChecker<T>();
            var prevState = checker.GetState((T) invocation.Request.Target);
            try
            {
                invocation.Proceed();
            }
            finally
            {
                var newState = checker.GetState((T) invocation.Request.Target);
                var changes = checker.GetChangedProperties(prevState, newState);

                foreach (var propertyName in changes)
                    RaisePropertyChanged(invocation.Request.Proxy, propertyName);
            }
        }

        private void RaisePropertyChanged(object proxy, string propertyName)
        {
            var autoNotify = proxy as IRaiseNotifyPropertyChanged;
            autoNotify?.RaiseNotifyPropertyChanged(propertyName);
        }
    }

    public static class InterceptorFactory
    {
        public static IInterceptor CreateNotifyPropertyChangedInterceptor(Type type)
        {
            var interceptorType = typeof (NotifyPropertyChangesInterceptor<>);
            var generic = interceptorType.MakeGenericType(type);

            return (IInterceptor)Activator.CreateInstance(generic);
        }
    }
}