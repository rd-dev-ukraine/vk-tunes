using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;

using Caliburn.Micro;

using Castle.DynamicProxy;

using Ninject;
using Ninject.Activation.Strategies;
using Ninject.Extensions.Interception.Infrastructure.Language;
using Ninject.Extensions.Interception.ProxyFactory;
using Ninject.Extensions.Interception.Wrapper;
using Ninject.Infrastructure.Language;

using VkTunes.Api.Api;
using VkTunes.Api.AudioStorage;
using VkTunes.Api.Authorization;
using VkTunes.Api.LowLevel;
using VkTunes.Api.Models;
using VkTunes.Api.Queue;
using VkTunes.Api.Throttle;
using VkTunes.AudioRecord;
using VkTunes.Configuration;
using VkTunes.Infrastructure;
using VkTunes.Infrastructure.AutoPropertyChange;
using VkTunes.Infrastructure.Navigation;
using VkTunes.IoC;
using VkTunes.Shell;
using VkTunes.Test;
using VkTunes.Utils;

namespace VkTunes
{
    public class Bootstrapper : BootstrapperBase
    {
        private readonly IKernel kernel = new StandardKernel();

        public Bootstrapper()
        {
            Initialize();

            kernel.Components.Add<IActivationStrategy, EventAggregatorSubscribeActivationStrategy>();
            kernel.Components.RemoveAll<IProxyFactory>();
            kernel.Components.Add<IProxyFactory, CustomProxyFactory>();

            kernel.Bind<ConfigurationReader>().ToSelf().InSingletonScope();
            kernel.Bind<ApplicationConfiguration>()
                  .ToFactoryMethod((ConfigurationReader reader) => reader.Read())
                  .InSingletonScope();
            kernel.Bind<Api.Configuration>()
                .ToFactoryMethod((ApplicationConfiguration appConfig) => appConfig.VkApi)
                .InSingletonScope();

            kernel.Bind<IAuthorization>().To<InAppBrowserAuthorization>();
            kernel.Bind<IAuthorizationInfo>().To<InMemoryAuthorizationInfo>().InSingletonScope();

            kernel.Bind<IVkHttpClient>().To<VkHttpClient>().InSingletonScope();
            kernel.Bind<IVkApi>().To<VkApi>().InSingletonScope();
            kernel.Bind<Vk>().ToSelf().InSingletonScope();
            kernel.Bind<IVkAudioFileStorage>().To<FileSystemAudioStorage>().InSingletonScope();

            kernel.Bind<IThrottler>().To<ParallelThrottlerSlim>().InSingletonScope();
            kernel.Bind<IApiRequestQueue>().To<PriorityApiRequestQueue>().InSingletonScope();
            kernel.Bind<INavigator>().To<Navigator>().InSingletonScope();
            kernel.Bind<IWindowManager>().To<WindowManager>().InSingletonScope();
            kernel.Bind<IEventAggregator>().To<EventAggregator>().InSingletonScope();
            kernel.BindFactory<AudioRecordViewModel>();

            kernel.Bind<TestViewModel>().ToSelf().Intercept(typeof(INotifyPropertyChanged), typeof(INotifyPropertyChangedEx))
                .With(request => InterceptorFactory.CreateNotifyPropertyChangedInterceptor(request.Context.Request.Service));

            kernel.Intercept(ctx => ctx.Request.Service.HasAttribute(typeof(AutoNotifyOnPropertyChangeAttribute)))
                  .With(request => InterceptorFactory.CreateNotifyPropertyChangedInterceptor(request.Context.Request.Service));


            //var originalTransformName = ViewLocator.TransformName;
            //ViewLocator.TransformName = (name, context) =>
            //{
            //    var result = originalTransformName(name, context);

            //    return result;
            //};

            //var oldLocateForModel = ViewLocator.LocateForModel;
            //ViewLocator.LocateForModel = (o, dependencyObject, arg3) =>
            //{
            //    var proxy = o as IProxyTargetAccessor;
            //    if (proxy == null)
            //    {
            //        if (proxy.GetType().Name.Contains("DynamicProxy"))
            //            return 
            //        return oldLocateForModel(o, dependencyObject, arg3);
            //    }
            //    else
            //    {
            //        var obj = (proxy.GetInterceptors()?[0] as DynamicProxyWrapper).Instance;
            //        return oldLocateForModel(obj, dependencyObject, arg3);
            //    }
            //};

            var oldLocate = ViewLocator.LocateTypeForModelType;
            ViewLocator.LocateTypeForModelType = (type, o, arg3) =>
            {
                if (type.FullName.StartsWith("NProxy"))
                {
                    var baseType = type.BaseType;
                    return oldLocate(baseType, o, arg3);
                }

                return oldLocate(type, o, arg3);
            };
        }

        protected override void OnStartup(object sender, StartupEventArgs e)
        {
            DisplayRootViewFor<TestViewModel>();
        }

        protected override object GetInstance(Type service, string key)
        {
            return kernel.Get(service, key);
        }

        protected override IEnumerable<object> GetAllInstances(Type service)
        {
            return kernel.GetAll(service);
        }
    }
}