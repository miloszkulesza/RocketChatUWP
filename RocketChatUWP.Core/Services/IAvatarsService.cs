using System.Threading.Tasks;
using Windows.UI.Xaml.Media;

namespace RocketChatUWP.Core.Services
{
    public interface IAvatarsService
    {
        ImageSource GetUserAvatar(string serverAddress, string username);
        ImageSource GetChannelAvatar(string serverAddress, string channelName);
        string GetUserAvatarUrl(string serverAddress, string username);
        string GetChannelAvatarUrl(string serverAddress, string channelName);
    }
}
