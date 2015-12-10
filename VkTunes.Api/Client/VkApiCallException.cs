using System;
using System.Runtime.Serialization;

namespace VkTunes.Api.Client
{
    public class VkApiCallException : Exception
    {
        public VkApiCallException()
        {
        }

        public VkApiCallException(string message) : base(message)
        {
        }

        public VkApiCallException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected VkApiCallException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}