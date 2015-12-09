using System;
using System.Collections.Specialized;
using System.Linq;

using VkTunes.Api.Url;

namespace VkTunes.Api.Authorization
{
    public class InAppBrowserAuthorization : IAuthorization
    {
        public static string VkAuthorizationUrl = "https://oauth.vk.com/authorize?v=5.21";
        public static string RedirectUrl = "https://oauth.vk.com/blank.html";

        private readonly Configuration configuration;
        private readonly IAuthorizationInfo authorizationInfo;

        public InAppBrowserAuthorization(Configuration configuration, IAuthorizationInfo authorizationInfo)
        {
            if (configuration == null)
                throw new ArgumentNullException(nameof(configuration));
            if (authorizationInfo == null)
                throw new ArgumentNullException(nameof(authorizationInfo));

            this.configuration = configuration;
            this.authorizationInfo = authorizationInfo;
        }

        public string AuthorizationUrl()
        {
            var parameters = new AuthorizationUrlParameters
            {
                AppId = configuration.AppId,
                Display = "touch",
                RedirectUri = UrlBuilder.EncodeUrl(RedirectUrl),
                ResponseType = "token",
                Scope = "audio"
            };

            return UrlBuilder.Build(VkAuthorizationUrl, parameters);
        }

        public bool ExtractTokenFromUrl(string url)
        {
            var urlMatch = RedirectUrl.Replace("https://", String.Empty);
            if (url.IndexOf(urlMatch, StringComparison.InvariantCultureIgnoreCase) != -1)
            {
                var query = ParseQueryString(url);

                authorizationInfo.Token = query["access_token"];
                authorizationInfo.UserId = Int32.Parse(query["user_id"]);

                return true;
            }

            return false;
        }

        private NameValueCollection ParseQueryString(string url)
        {
            if (String.IsNullOrWhiteSpace(url))
                throw new ArgumentNullException(nameof(url));

            // Return empty collection if there is no query string
            var urlParts = url.Split('#');
            if (urlParts.Length == 1)
                return new NameValueCollection();

            var queryString = urlParts[1];

            var nameValuePairs = queryString.Split('&');

            var pairs = nameValuePairs.Select(p => p.Split('='))
                .Select(p => new
                {
                    Name = p[0],
                    Value = p.Length > 1 ? p[1] : String.Empty
                })
                .Select(a => new
                {
                    Name = Uri.UnescapeDataString(a.Name),
                    Value = Uri.UnescapeDataString(a.Value)
                })
                .ToArray();

            var result = new NameValueCollection(StringComparer.InvariantCultureIgnoreCase);

            foreach (var p in pairs)
                result.Add(p.Name, p.Value);

            return result;
        }
    }
}