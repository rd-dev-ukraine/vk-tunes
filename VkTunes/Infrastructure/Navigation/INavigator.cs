using Caliburn.Micro;

namespace VkTunes.Infrastructure.Navigation
{
    public interface INavigator
    {
        void GoTo<TScreen>() where TScreen : IScreen;
    }
}