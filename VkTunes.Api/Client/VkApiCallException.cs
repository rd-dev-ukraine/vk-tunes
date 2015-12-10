using System;
using System.Runtime.Serialization;

namespace VkTunes.Api
{
    /// <summary>
    /// The exception that is thrown if call vk.com API doesn't return expected results because of parameters are invalid.
    /// </summary>
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