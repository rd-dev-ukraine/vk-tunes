using System.Threading.Tasks;

namespace VkTunes.Api.Client
{
    public interface IVk
    {
        Task<UserAudioResponse> MyAudio();
    }
}