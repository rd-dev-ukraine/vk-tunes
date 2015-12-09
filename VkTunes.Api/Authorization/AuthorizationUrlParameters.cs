using VkTunes.Api.Url;

namespace VkTunes.Api.Authorization
{
    public class AuthorizationUrlParameters
    {
        [QueryStringName("client_id")]
        public string AppId { get; set; } 

        [QueryStringName("scope")]
        public string Scope { get; set; }

        [QueryStringName("redirect_uri")]
        public string RedirectUri { get; set; }

        [QueryStringName("response_type")]
        public string ResponseType { get; set; }

        [QueryStringName("display")]
        public string Display { get; set; }
    }
}