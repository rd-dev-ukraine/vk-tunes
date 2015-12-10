using System;
using System.Runtime.Serialization;

namespace VkTunes.Api.LowLevel
{
    /// <summary>
    /// The exception that is thrown if call to vk.com API was unsuccessful.
    /// </summary>
    public abstract class ApiCallException : Exception
    {
        protected ApiCallException()
        {
        }

        protected ApiCallException(string message) : base(message)
        {
        }

        protected ApiCallException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected ApiCallException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}