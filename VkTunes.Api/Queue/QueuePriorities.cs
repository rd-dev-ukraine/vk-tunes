namespace VkTunes.Api.Queue
{
    public static class QueuePriorities
    {
        public const int GetFileSize = 1;
        public const int DownloadFile = 10;
        public const int ApiCall = 100;

        public const int ApiCallSearchAudio = ApiCall + 1;
    }
}