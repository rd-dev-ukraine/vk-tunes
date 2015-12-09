using System;
using System.Linq;
using System.Threading.Tasks;

using Caliburn.Micro;

using VkTunes.Api.Client;
using VkTunes.AudioRecord;
using VkTunes.Infrastructure.Async;

namespace VkTunes
{
    public abstract class AudioListModelBase: Screen
    {
        protected AudioListModelBase(IVk vk, IAsync @async)
        {
            if (vk == null)
                throw new ArgumentNullException(nameof(vk));
            if (async == null)
                throw new ArgumentNullException(nameof(async));

            Vk = vk;
            Async = async;
        }

        protected IVk Vk { get; }

        private IAsync Async { get; }

        public BindableCollection<AudioRecordViewModel> Audio { get; set; } = new BindableCollection<AudioRecordViewModel>();

        protected abstract Task<UserAudioResponse> LoadAudio();

        public void Reload()
        {
            Async.Execute(LoadAudio, r =>
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