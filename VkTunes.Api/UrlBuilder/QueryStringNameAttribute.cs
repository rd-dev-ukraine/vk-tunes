using System;

namespace VkTunes.Api.UrlBuilder
{
    [AttributeUsage(AttributeTargets.Property)]
    public class QueryStringNameAttribute : Attribute
    {
        public QueryStringNameAttribute(string queryStringName)
        {
            if (String.IsNullOrWhiteSpace(queryStringName))
                throw new ArgumentNullException(nameof(queryStringName));

            QueryStringName = queryStringName;
        }

        public string QueryStringName { get; set; }
    }
}