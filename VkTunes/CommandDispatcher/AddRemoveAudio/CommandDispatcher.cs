using System.Threading.Tasks;

using Caliburn.Micro;

using VkTunes.CommandDispatcher.AddRemoveAudio;
using VkTunes.CommandDispatcher.AudioCommon;

// ReSharper disable once CheckNamespace
namespace VkTunes.CommandDispatcher
{
    public partial class CommandDispatcher : IHandleWithTask<AddToMyAudioCommand>
    {
        public async Task Handle(AddToMyAudioCommand message)
        {
            var record = await vk.AddAudio(message.AudioId, message.OwnerId);
            if (record != null)
            {
                await Task.WhenAll(
                        Event(new RemoteAudioUpdatedEvent(record)), 
                        Event(new MyAudioAddedEvent(record)));
            }
        }
    }
}