using System;
using System.Collections.Generic;
using System.Windows;

using Caliburn.Micro;

using Ninject;

using VkTunes.Api;
using VkTunes.Api.Queue;
using VkTunes.Configuration;
using VkTunes.Infrastructure.Async;
using VkTunes.Infrastructure.Navigation;
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

            kernel.Load(new ApiModule());

            kernel.Bind<IAsync>().To<AsyncRunner>().InSingletonScope();
            kernel.Bind<INavigator>().To<Navigator>().InSingletonScope();
            kernel.Bind<IWindowManager>().To<WindowManager>().InSingletonScope();
            kernel.Bind<IEventAggregator>().To<EventAggregator>().InSingletonScope();
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