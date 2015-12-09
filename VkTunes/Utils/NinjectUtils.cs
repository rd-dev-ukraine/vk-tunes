using System;

using Ninject;
using Ninject.Syntax;

namespace VkTunes.Utils
{
    public static class NinjectUtils
    {
        public static IBindingWhenInNamedWithOrOnSyntax<T1> ToFactoryMethod<T1, TFirstParam>(this IBindingToSyntax<T1> binding, Func<TFirstParam, T1> factory)
        {
            return binding.ToMethod(context =>
            {
                var firstParam = (TFirstParam)context.Kernel.Get(typeof (TFirstParam));

                return factory(firstParam);
            });
        }
    }
}