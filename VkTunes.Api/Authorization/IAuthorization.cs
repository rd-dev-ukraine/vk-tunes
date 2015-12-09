namespace VkTunes.Api.Authorization
{
    public interface IAuthorization
    {
        string AuthorizationUrl();

        bool ExtractTokenFromUrl(string url);
    }
}