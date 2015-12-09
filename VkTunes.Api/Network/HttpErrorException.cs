using System;

namespace VkTunes.Api.Network
{
    public class HttpErrorException: Exception
    {
        public HttpErrorException()
        {
        }

        public HttpErrorException(string message) : base(message)
        {
        }

        public HttpErrorException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}