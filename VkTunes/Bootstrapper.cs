using System;
using System.Collections.Generic;
using System.Windows;

using Caliburn.Micro;

using Ninject;
using Ninject.Extensions.Interception.Infrastructure.Language;

using VkTunes.Api.Api;
using VkTunes.Api.AudioStorage;
using VkTunes.Api.Authorization;
using VkTunes.Api.LowLevel;
using VkTunes.Api.Models;
using VkTunes.Api.Queue;
using VkTunes.Api.Throttle;
using VkTunes.AudioRecord;
using VkTunes.Configuration;
using VkTunes.Infrastructure.AutoPropertyChange;
using VkTunes.Infrastructure.Navigation;
using VkTunes.IoC;
using VkTunes.Shell;
using VkTunes.Utils;

namespace VkTunes
{
    public class Bootstrapper : BootstrapperBase
    {
        private readonly IKernel kernel = new StandardKernel();

        public Bootstrapper()
        {
            Initialize();

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

            kernel.Intercept(ctx => typeof(IRaiseNotifyPropertyChanged).IsAssignableFrom(ctx.Request.Service))
                  .With(request => InterceptorFactory.CreateNotifyPropertyChangedInterceptor(request.Context.Request.Service));
        }

        protected override void OnStartup(object sender, StartupEventArgs e)
        {
            DisplayRootViewFor<ShellViewModel>();
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