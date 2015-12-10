using Newtonsoft.Json;

namespace VkTunes.Api.LowLevel
{
    public class VkApiErrorDetails
    {
        [JsonProperty("error_code")]
        public int ErrorCode { get; set; }

        [JsonProperty("error_msg")]
        public string ErrorMessage { get; set; }
    }
}