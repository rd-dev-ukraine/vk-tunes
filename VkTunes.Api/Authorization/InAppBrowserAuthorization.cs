using System;

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

        public string AuthorizationUrl
        {
            get
            {
                throw new NotImplementedException();
            }
        }
    }
}