using System.Threading.Tasks;

using VkTunes.Api.Client;
using VkTunes.Infrastructure.Async;

namespace VkTunes.AudioList
{
    public class AudioListViewModel : AudioListModelBase
    {
        public AudioListViewModel(IVk vk, IAsync async) : base(vk, async)
        {
        }

        protected override void OnActivate()
        {
            base.OnActivate();
            Reload();
        }

        protected override async Task<UserAudioResponse> LoadAudio()
        {
            return await Vk.MyAudio();
        }
    }
}