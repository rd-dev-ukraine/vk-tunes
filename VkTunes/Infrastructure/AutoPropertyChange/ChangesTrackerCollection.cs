using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace VkTunes.Infrastructure.AutoPropertyChange
{
    public class ChangesTrackerCollection
    {
        private readonly Dictionary<object, IChangesTracker> trackers = new Dictionary<object, IChangesTracker>();

        public void BeginTrack(object instance)
        {
            if (instance == null)
                throw new ArgumentNullException(nameof(instance));

            var tracker = EnsureTracker(instance);
            tracker.BeginTrackingScope();
        }

        public void CompleteTrack(object instance, PropertyChangedEventHandler notifyPropertyChanged)
        {
            if (instance == null)
                throw new ArgumentNullException(nameof(instance));

            var tracker = EnsureTracker(instance);
            tracker.EndTrackingScope(notifyPropertyChanged);
        }

        private IChangesTracker EnsureTracker(object instance)
        {
            if (instance == null)
                throw new ArgumentNullException(nameof(instance));

            IChangesTracker tracker;
            if (!trackers.TryGetValue(instance, out tracker))
            {
                tracker = CreateTracker(instance);
                trackers[instance] = tracker;
            }

            return tracker;
        }

        private IChangesTracker CreateTracker(object instance)
        {
            if (instance == null)
                throw new ArgumentNullException(nameof(instance));

            var type = typeof (ChangesTracker<>).MakeGenericType(instance.GetType());
            return (IChangesTracker) Activator.CreateInstance(type, instance);
        }
    }
}