using System;

namespace VkTunes.Infrastructure.Navigation
{
    public class GoToViewModelEvent
    {
        public Type ViewModelType { get; set; }
    }
}