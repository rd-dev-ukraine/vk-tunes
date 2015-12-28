using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace VkTunes.Infrastructure.AutoPropertyChange
{
    public class ChangesTracker<T> : INotifyPropertyChanged
        where T : class
    {
        private readonly T target;
        private readonly DirtyChecker<T> dirtyChecker = new DirtyChecker<T>();

        private int nest;
        private IDictionary<string, object> state;

        public ChangesTracker(T target)
        {
            if (target == null)
                throw new ArgumentNullException(nameof(target));

            this.target = target;
        }

        public void BeginTrackingScope()
        {
            if (nest == 0)
                state = dirtyChecker.GetState(target);

            nest++;
        }

        public void EndTrackingScope()
        {
            nest--;

            if (nest == 0)
            {
                var newState = dirtyChecker.GetState(target);

                var changes = dirtyChecker.GetChangedProperties(state, newState);
                foreach(var property in changes)
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(property));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}