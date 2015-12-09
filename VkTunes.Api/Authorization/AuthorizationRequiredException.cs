using System;

namespace VkTunes.Api.Authorization
{
    public class AuthorizationRequiredException : Exception
    {
        public AuthorizationRequiredException()
        {
        }

        public AuthorizationRequiredException(string message) : base(message)
        {
        }

        public AuthorizationRequiredException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}