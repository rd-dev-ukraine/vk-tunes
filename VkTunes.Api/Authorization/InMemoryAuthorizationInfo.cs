namespace VkTunes.Api.Authorization
{
    public class InMemoryAuthorizationInfo : IAuthorizationInfo
    {
        public string Token { get; set; }

        public int UserId { get; set; }
    }
}