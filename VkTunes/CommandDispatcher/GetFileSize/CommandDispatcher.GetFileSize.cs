using System;
using System.Threading.Tasks;

using Caliburn.Micro;

using VkTunes.CommandDispatcher.GetFileSize;

// ReSharper disable once CheckNamespace
namespace VkTunes.CommandDispatcher
{
    public partial class CommandDispatcher : IHandleWithTask<UpdateRemoteFileSizeCommand>
    {
        public async Task Handle(UpdateRemoteFileSizeCommand message)
        {
            if (!String.IsNullOrWhiteSpace(message.FileUrl))
            {
                var size = await vk.GetFileSize(message.FileUrl);

                if (size != null)
                    await Event(new RemoteFileSizeUpdatedEvent(message.AudioId, message.OwnerId, size.Value));
            }
        }
    }
}