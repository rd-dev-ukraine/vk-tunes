using Ninject.Modules;

using VkTunes.Api.Api;
using VkTunes.Api.AudioStorage;
using VkTunes.Api.Authorization;
using VkTunes.Api.LowLevel;
using VkTunes.Api.Models;
using VkTunes.Api.Queue;
using VkTunes.Api.Throttle;

namespace VkTunes.Api
{
    public class ApiModule : NinjectModule
    {
        public override void Load()
        {
            Bind<IAuthorization>().To<InAppBrowserAuthorization>();
            Bind<IAuthorizationInfo>().To<InMemoryAuthorizationInfo>().InSingletonScope();

            Bind<IVkHttpClient>().To<VkHttpClient>().InSingletonScope();
            Bind<IVkApi>().To<VkApi>().InSingletonScope();
            Bind<Vk>().ToSelf().InSingletonScope();
            Bind<IVkAudioFileStorage>().To<FileSystemAudioStorage>().InSingletonScope();

            Bind<IThrottler>().To<SlimThrottler>().InSingletonScope();
            Bind<IApiRequestQueue>().To<PriorityApiRequestQueue>().InSingletonScope();
        }
    }
}