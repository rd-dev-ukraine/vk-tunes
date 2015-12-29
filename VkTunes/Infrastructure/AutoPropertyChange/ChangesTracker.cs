using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace VkTunes.Infrastructure.AutoPropertyChange
{
    public interface IChangesTracker
    {
        void BeginTrackingScope();

        void EndTrackingScope(PropertyChangedEventHandler propertyChanged);
    }

    public class ChangesTracker<T> : IChangesTracker where T : class
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
            {
                nest++;
                state = dirtyChecker.GetState(target);
            }
            else
                nest++;
        }

        public void EndTrackingScope(PropertyChangedEventHandler propertyChanged)
        {
            if (nest == 1)
            {
                var newState = dirtyChecker.GetState(target);

                var changes = dirtyChecker.GetChangedProperties(state, newState);
                foreach (var property in changes)
                    propertyChanged?.Invoke(this, new PropertyChangedEventArgs(property));
            }
            nest--;
        }
    }
}