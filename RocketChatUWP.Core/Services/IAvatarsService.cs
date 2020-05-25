using System.Threading.Tasks;
using Windows.UI.Xaml.Media;

namespace RocketChatUWP.Core.Services
{
    public interface IAvatarsService
    {
        Task<ImageSource> GetAvatar(string address);
    }
}
