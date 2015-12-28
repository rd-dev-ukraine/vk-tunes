using System.ComponentModel;

namespace VkTunes.Infrastructure.AutoPropertyChange
{
    public interface IRaiseNotifyPropertyChanged : INotifyPropertyChanged
    {
        void RaiseNotifyPropertyChanged(string propertyName);
    }
}