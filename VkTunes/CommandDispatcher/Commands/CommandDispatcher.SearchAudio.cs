using System;
using System.Linq;
using System.Threading.Tasks;

using Caliburn.Micro;

using VkTunes.Api.Models;
using VkTunes.Api.Queue;

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

    public class SearchAudioCommand : CommandBase
    {
        public SearchAudioCommand(string query)
        {
            Query = query;
        }

        public string Query { get; }
    }

    public class SearchAudioResultReceivedEvent : EventBase
    {
        public SearchAudioResultReceivedEvent(AudioInfo[] audio, string query)
        {
            if (audio == null)
                throw new ArgumentNullException(nameof(audio));

            Audio = audio;
            Query = query;
        }

        public string Query { get; }

        public AudioInfo[] Audio { get; }
    }
}