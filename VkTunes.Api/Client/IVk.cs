using System.Threading.Tasks;

namespace VkTunes.Api.Client
{
    public interface IVk
    {
        Task<UserAudioResponse> MyAudio();

        Task<int> FileSize(string url);
    }
}