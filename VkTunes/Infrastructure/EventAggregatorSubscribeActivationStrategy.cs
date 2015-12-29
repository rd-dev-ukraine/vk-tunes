using System.ComponentModel;
using System.Diagnostics;

using Caliburn.Micro;

using Ninject;
using Ninject.Activation;
using Ninject.Activation.Strategies;

namespace VkTunes.Infrastructure
{
    public class EventAggregatorSubscribeActivationStrategy : IActivationStrategy
    {
        public INinjectSettings Settings { get; set; }

        public void Activate(IContext context, InstanceReference reference)
        {
            reference.IfInstanceIs<IHandle>(x =>
            {
                var ea = context.Kernel.Get<IEventAggregator>();
                ea.Subscribe(reference.Instance);
            });
        }

        public void Deactivate(IContext context, InstanceReference reference)
        {
            reference.IfInstanceIs<IHandle>(x =>
            {
                var ea = context.Kernel.Get<IEventAggregator>();
                ea.Unsubscribe(reference.Instance);
            });
        }

        public void Dispose()
        {
        }
    }
}