using System.Threading.Tasks;

namespace VkTunes.Api.Queue
{
    public interface IQueueItem
    {
        int Priority { get; }

        Task Run();
    }
}