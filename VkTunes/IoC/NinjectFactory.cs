using System;

using Ninject;

namespace VkTunes.IoC
{
    public class NinjectFactory<TFactory>: IFactory<TFactory>
    {
        private readonly IKernel kernel;

        public NinjectFactory(IKernel kernel)
        {
            if (kernel == null)
                throw new ArgumentNullException(nameof(kernel));

            this.kernel = kernel;
        }

        public TFactory CreateInstance()
        {
            return kernel.Get<TFactory>();
        }
    }
}