using System;
using System.Linq;
using System.Threading.Tasks;

using Caliburn.Micro;

using VkTunes.Api.Models;

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

    public class MyAudioLoadCommand : CommandBase
    {

    }

    public class MyAudioLoadedEvent : EventBase
    {
        public MyAudioLoadedEvent(AudioInfo[] audio)
        {
            if (audio == null)
                throw new ArgumentNullException(nameof(audio));

            Audio = audio;
        }

        public AudioInfo[] Audio { get; }
    }
}