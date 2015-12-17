using System.Linq;
using System.Threading.Tasks;

using Caliburn.Micro;

using VkTunes.Api.Queue;
using VkTunes.CommandDispatcher.SearchAudio;

// ReSharper disable once CheckNamespace
namespace VkTunes.CommandDispatcher
{
    public partial class CommandDispatcher : IHandleWithTask<SearchAudioCommand>
    {
        public async Task Handle(SearchAudioCommand message)
        {
            vk.CancelTasks(QueuePriorities.ApiCallSearchAudio);
            var searchResults = await LoadAudioCollection(async () => (await vk.SearchAudio(message.Query)).Audio);
            await PublishEvent(new SearchAudioResultReceivedEvent(searchResults.ToArray(), message.Query));
        }
    }
}