namespace VkTunes.Api.LowLevel
{
    /// <summary>
    /// The exception that is thrown when calling vk.com API returns error in response body.
    /// </summary>
    public class ErrorApiCallException : ApiCallException
    {
        public ErrorApiCallException(VkApiErrorDetails errorDetails)
            : base(errorDetails.ErrorMessage)
        {
            ErrorDetails = errorDetails;
        }

        public VkApiErrorDetails ErrorDetails { get; set; }
    }
}