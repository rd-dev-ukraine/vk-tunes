using VkTunes.Api.Url;

namespace VkTunes.Api.LowLevel
{
    public class VkApiRequestParameters
    {
        [QueryStringName("access_token")]
        public string AccessToken { get; set; }

        [QueryStringName("v")]
        public string Version { get; set; }
    }
}