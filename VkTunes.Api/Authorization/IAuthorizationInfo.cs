namespace VkTunes.Api.Authorization
{
    public interface IAuthorizationInfo
    {
        string Token { get; set; } 

        int UserId { get; set; }
    }
}