using System;

namespace VkTunes.Api.LowLevel
{
    public class HttpErrorApiCallException : ApiCallException
    {
        public HttpErrorApiCallException(string method, string url)
            : base($"Error requesting {method} {url}")
        {
            Method = method;
            Url = url;
        }

        public HttpErrorApiCallException(string method, string url, string message)
            : base(message)
        {
            Method = method;
            Url = url;
        }

        public HttpErrorApiCallException(string method, string url, string message, Exception innerException)
            : base(message, innerException)
        {
            Method = method;
            Url = url;
        }

        public string Method { get; }

        public string Url { get; }
    }
}