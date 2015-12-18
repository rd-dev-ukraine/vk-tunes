using VkTunes.Api.Url;

namespace VkTunes.Api.Api
{
    public class SearchAudioRequest
    {
        [QueryStringName("q")]
        public string Query { get; set; }

        /// <summary>
        /// 1 - true, 0 - false
        /// </summary>
        [QueryStringName("auto_complete")]
        public int AutoComplete { get; set; } = 1;

        /// <summary>
        /// 1 - true, 0 - false
        /// </summary>
        [QueryStringName("search_own")]
        public int SearchOwnAudio { get; set; } = 1;

        [QueryStringName("count")]
        public int ResultsCount { get; set; } = 100;
    }
}