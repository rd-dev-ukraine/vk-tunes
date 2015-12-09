using Caliburn.Micro;

namespace VkTunes.Infrastructure
{
    public interface INavigator
    {
        void GoTo<TScreen>() where TScreen : IScreen;
    }
}