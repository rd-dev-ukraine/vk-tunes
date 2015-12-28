using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace VkTunes.Infrastructure.AutoPropertyChange
{
    /// <summary>
    /// Finds changed properties
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class DirtyChecker<T>
        where T : class
    {
        private static readonly IDictionary<string, Func<T, object>> Accessor = BuildAccessor();

        public IDictionary<string, object> GetState(T instance)
        {
            if (instance == null)
                throw new ArgumentNullException(nameof(instance));

            return Accessor.ToDictionary(k => k.Key, k => k.Value(instance));
        }

        public ISet<string> GetChangedProperties(IDictionary<string, object> previousState, IDictionary<string, object> newState)
        {
            if (previousState == null)
                throw new ArgumentNullException(nameof(previousState));
            if (newState == null)
                throw new ArgumentNullException(nameof(newState));

            var result = new HashSet<string>(StringComparer.Ordinal);

            foreach (var kvp in Accessor)
            {
                object previousValue = null;
                object newValue = null;

                previousState.TryGetValue(kvp.Key, out previousValue);
                newState.TryGetValue(kvp.Key, out newValue);

                if (!EqualityComparer<object>.Default.Equals(previousValue, newValue))
                    result.Add(kvp.Key);
            }

            return result;
        }

        private static IDictionary<string, Func<T, object>> BuildAccessor()
        {
            return typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.FlattenHierarchy)
                .Where(p => p.CanRead)
                .ToDictionary(
                    p => p.Name,
                    p =>
                    {
                        var parameter = Expression.Parameter(typeof(T));
                        return Expression.Lambda<Func<T, object>>(
                                        Expression.Convert(
                                            Expression.Property(parameter, p),
                                            typeof(object)),
                                        parameter)
                                   .Compile();
                    });
        }
    }
}