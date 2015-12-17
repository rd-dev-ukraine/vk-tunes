using System.Linq;
using System.Threading.Tasks;

using Caliburn.Micro;

using VkTunes.CommandDispatcher.MyAudio;

// ReSharper disable once CheckNamespace
namespace VkTunes.CommandDispatcher
{
    public partial class CommandDispatcher : IHandleWithTask<MyAudioLoadCommand>
    {
        public async Task Handle(MyAudioLoadCommand message)
        {
            var myAudio = await LoadAudioCollection(async () => (await vk.MyAudio()).Audio);
            await PublishEvent(new MyAudioLoadedEvent(myAudio.ToArray()));
        }
    }
}