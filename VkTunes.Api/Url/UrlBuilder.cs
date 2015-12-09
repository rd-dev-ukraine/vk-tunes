using System;
using System.Linq;
using System.Reflection;

namespace VkTunes.Api.Url
{
    public static class UrlBuilder
    {
        public static string SerializeToQueryString<T>(T parameters)
            where T : class
        {
            if (parameters == null)
                throw new ArgumentNullException(nameof(parameters));

            var props = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.FlattenHierarchy);

            var nameValue = props.Where(p => p.IsDefined(typeof(QueryStringNameAttribute)))
                .Select(p => new
                {
                    Name = p.GetCustomAttribute<QueryStringNameAttribute>().QueryStringName,
                    Value = Convert.ToString(p.GetValue(parameters))
                });

            return String.Join(
                "&",
                nameValue.Select(a => new { Name = Uri.EscapeDataString(a.Name), Value = Uri.EscapeDataString(a.Value) })
                    .Select(a => $"{a.Name}={a.Value}"));
        }

        public static string Build<T>(string path, T query)
            where T : class
        {
            if (String.IsNullOrWhiteSpace(path))
                throw new ArgumentNullException(nameof(path));
            if (query == null)
                throw new ArgumentNullException(nameof(query));

            var queryString = SerializeToQueryString(query);
            path = path.TrimEnd('?');


            return path.Contains("?") ? path + "&" + queryString : path + "?" + queryString;
        }

        public static string EncodeUrl(string url)
        {
            return Uri.EscapeUriString(url);
        }
    }
}