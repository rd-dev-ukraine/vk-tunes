namespace VkTunes.CommandDispatcher.SearchAudio
{
    public class SearchAudioCommand : CommandBase
    {
        public SearchAudioCommand(string query)
        {
            Query = query;
        }

        public string Query { get; }
    }
}