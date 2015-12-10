using Ninject.Modules;

using VkTunes.Api.AudioStorage;
using VkTunes.Api.Authorization;
using VkTunes.Api.LowLevel;
using VkTunes.Api.Queue;

namespace VkTunes.Api
{
    public class ApiModule : NinjectModule
    {
        public override void Load()
        {
            Bind<IAuthorization>().To<InAppBrowserAuthorization>();
            Bind<IAuthorizationInfo>().To<InMemoryAuthorizationInfo>().InSingletonScope();

            Bind<IVkApiClient>().To<VkApiHttpClient>().InSingletonScope();
            Bind<IVk>().To<Vk>().InSingletonScope();
            Bind<IVkAudioFileStorage>().To<FileSystemAudioStorage>().InSingletonScope();

            Bind<IApiRequestQueue>().To<VkPriorityApiRequestQueue>().InSingletonScope();
        }
    }
}