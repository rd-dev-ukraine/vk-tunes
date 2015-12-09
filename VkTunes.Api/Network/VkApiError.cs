using Newtonsoft.Json;

namespace VkTunes.Api.Network
{
    public class VkApiError
    {
        [JsonProperty("error")]
        public VkApiErrorDetails Error { get; set; }
    }

    public class VkApiErrorDetails
    {
        [JsonProperty("error_code")]
        public int ErrorCode { get; set; }

        [JsonProperty("error_msg")]
        public string ErrorMessage { get; set; }
    }
}