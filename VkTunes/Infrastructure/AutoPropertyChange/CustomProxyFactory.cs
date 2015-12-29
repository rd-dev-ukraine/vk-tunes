using System;
using System.Collections.Generic;
using System.Linq;

using Ninject;
using Ninject.Activation;
using Ninject.Extensions.Interception.ProxyFactory;
using Ninject.Infrastructure;
using Ninject.Selection.Heuristics;

using NProxy.Core;
using NProxy.Core.Interceptors;

namespace VkTunes.Infrastructure.AutoPropertyChange
{
    public class CustomProxyFactory : ProxyFactoryBase, IHaveKernel
    {
        private readonly ProxyFactory factory = new ProxyFactory(new NotifyPropertyChangedInterceptionFilter());

        public CustomProxyFactory(IKernel kernel)
        {
            Kernel = kernel;
        }

        public IKernel Kernel { get; }

        public override void Wrap(IContext context, InstanceReference reference)
        {
            reference.Instance = CreateProxy(context, reference);
        }

        public override void Unwrap(IContext context, InstanceReference reference)
        {
        }

        private object CreateProxy(IContext context, InstanceReference reference)
        {
            var template = factory.GetProxyTemplate(context.Request.Service, context.Request.Service.GetInterfaces());
            var ctor = context.Request.Service.GetConstructors().FirstOrDefault();

            var args = new object[] {};

            if (ctor != null)
                args = ctor.GetParameters().Select(p => context.Kernel.Get(p.ParameterType)).ToArray();

            return template.CreateProxy(new NotifyPropertyChangesInvokationHandler(), args);
        }
    }
}