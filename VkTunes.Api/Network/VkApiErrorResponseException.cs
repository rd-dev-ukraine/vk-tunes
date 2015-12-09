using System;

namespace VkTunes.Api.Network
{
    public class VkApiErrorResponseException : Exception
    {
        public VkApiErrorResponseException(VkApiErrorDetails errorDetails)
            : base(errorDetails.ErrorMessage)
        {
            ErrorDetails = errorDetails;
        }

        public VkApiErrorDetails ErrorDetails { get; set; }
    }
}