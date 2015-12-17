using System;
using System.Threading.Tasks;

using Caliburn.Micro;

using VkTunes.Api.AudioStorage;
using VkTunes.Api.Models;

namespace VkTunes.CommandDispatcher
{
    public partial class CommandDispatcher
    {
        private readonly IEventAggregator eventAggregator;
        private readonly Vk vk;
        private readonly IVkAudioFileStorage storage;

        public CommandDispatcher(
            IEventAggregator eventAggregator,
            Vk vk,
            IVkAudioFileStorage storage)
        {
            if (eventAggregator == null)
                throw new ArgumentNullException(nameof(eventAggregator));
            if (vk == null)
                throw new ArgumentNullException(nameof(vk));
            if (storage == null)
                throw new ArgumentNullException(nameof(storage));

            this.eventAggregator = eventAggregator;
            this.vk = vk;
            this.storage = storage;

            eventAggregator.Subscribe(this);
        }

        private Task PublishEvent<TEvent>(TEvent @event)
            where TEvent : EventBase
        {
            if (@event == null)
                throw new ArgumentNullException(nameof(@event));

            return eventAggregator.PublishOnUIThreadAsync(@event);
        }
    }
}