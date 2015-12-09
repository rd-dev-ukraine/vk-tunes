using System;
using System.Linq;

using Caliburn.Micro;

using VkTunes.Api.Client;
using VkTunes.Infrastructure.Async;

namespace VkTunes.AudioList
{
    public class AudioListViewModel : Screen
    {
        private readonly IVk vk;
        private readonly IAsync async;

        public AudioListViewModel(IVk vk, IAsync @async)
        {
            if (vk == null)
                throw new ArgumentNullException(nameof(vk));
            if (async == null)
                throw new ArgumentNullException(nameof(async));

            this.vk = vk;
            this.async = async;
        }

        public BindableCollection<AudioRecordViewModel> Audio { get; set; } = new BindableCollection<AudioRecordViewModel>();

        protected override void OnActivate()
        {
            base.OnActivate();
            Reload();
        }

        public void Reload()
        {
            @async.Execute(() => vk.MyAudio(), r =>
            {
                Audio.Clear();
                Audio.AddRange(r.Audio.Select(Map));
            });
        }

        private AudioRecordViewModel Map(Api.Client.AudioRecord record)
        {
            return new AudioRecordViewModel
            {
                Id = record.Id,
                Duration = TimeSpan.FromSeconds(record.DurationInSeconds).ToString(),
                Title = $"{record.Artist} - {record.Title}"
            };
        }
    }
}