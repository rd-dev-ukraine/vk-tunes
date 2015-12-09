using Newtonsoft.Json;

namespace VkTunes.Api.Client
{
    public class AudioRecord
    {
        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("owner_id")]
        public int Owner { get; set; }

        [JsonProperty("artist")]
        public string Artist { get; set; }

        [JsonProperty("title")]
        public string Title { get; set; }

        [JsonProperty("duration")]
        public int DurationInSeconds { get; set; }

        [JsonProperty("url")]
        public string FileUrl { get; set; }

        [JsonProperty("lyrics_id")]
        public int LyricsId { get; set; }

        [JsonProperty("album_id")]
        public int AlbumId { get; set; }

        [JsonProperty("genre_id")]
        public int GenreId { get; set; }
    }
}