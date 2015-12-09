using System;

namespace VkTunes.Infrastructure.Navigation
{
    public class GoToViewModelEvent
    {
        public Type ViewModel { get; set; }
    }
}