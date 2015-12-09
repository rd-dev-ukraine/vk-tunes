using Ninject.Modules;

using VkTunes.Api.Authorization;

namespace VkTunes.Api
{
    public class ApiModule : NinjectModule
    {
        public override void Load()
        {
            Bind<IAuthorization>().To<InAppBrowserAuthorization>();
        }
    }
}