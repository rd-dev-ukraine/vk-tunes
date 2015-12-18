using Ninject;

namespace VkTunes.IoC
{
    public static class NinjectExtension
    {
        public static void BindFactory<TInstance>(this IKernel kernel)
        {
            kernel.Bind<IFactory<TInstance>>().To<NinjectFactory<TInstance>>().InSingletonScope();
            kernel.Bind<TInstance>().ToSelf().InTransientScope();
        }
    }
}