namespace VkTunes.Api.AudioStorage
{
    public class LocalAudioRecord
    {
        public int Id { get; set; }

        public int OwnerId { get; set; }

        public string Name { get; set; }

        public string FilePath { get; set; }
    }
}