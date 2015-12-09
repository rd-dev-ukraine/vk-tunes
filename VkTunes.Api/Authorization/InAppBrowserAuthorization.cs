using System;

using VkTunes.Api.Url;

namespace VkTunes.Api.Authorization
{
    public class InAppBrowserAuthorization : IAuthorization
    {
        private readonly Configuration configuration;

        public InAppBrowserAuthorization(Configuration configuration)
        {
            if (configuration == null)
                throw new ArgumentNullException(nameof(configuration));

            this.configuration = configuration;
        }

        public string AuthorizationUrl()
        {
            var parameters = new AuthorizationUrlParameters
            {
                AppId = configuration.AppId,
                Display = "touch",
                RedirectUri = UrlBuilder.EncodeUrl(Urls.ValidReturnUrl),
                ResponseType = "token",
                Scope = "audio"
            };

            return UrlBuilder.Build(Urls.AuthorizationUrl, parameters);
        }
    }
}