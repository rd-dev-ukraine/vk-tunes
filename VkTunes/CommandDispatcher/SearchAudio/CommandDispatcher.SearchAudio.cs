using System.Linq;

using Caliburn.Micro;

using VkTunes.Api.Queue;
using VkTunes.CommandDispatcher.SearchAudio;

// ReSharper disable once CheckNamespace
namespace VkTunes.CommandDispatcher
{
    public partial class CommandDispatcher : IHandle<SearchAudioCommand>
    {
        public void Handle(SearchAudioCommand message)
        {
            Throttle(message, async m =>
            {
                vk.CancelTasks(QueuePriorities.ApiCallSearchAudio);
                var searchResults = await LoadAudioCollection(async () => (await vk.SearchAudio(m.Query)).Audio);
                await PublishEvent(new SearchAudioResultReceivedEvent(searchResults.ToArray(), m.Query));
            });
        }
    }
}