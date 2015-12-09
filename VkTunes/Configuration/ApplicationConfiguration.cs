namespace VkTunes.Configuration
{
    public class ApplicationConfiguration
    {
        public ApplicationConfiguration()
        {
            VkApi = new Api.Configuration();
        }

        public Api.Configuration VkApi { get; set; } 
    }
}