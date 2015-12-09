using VkTunes.Api.Url;

namespace VkTunes.Api.Network
{
    public class VkApiRequestParameters
    {
        [QueryStringName("access_token")]
        public string AccessToken { get; set; }

        [QueryStringName("v")]
        public string Version { get; set; }
    }
}