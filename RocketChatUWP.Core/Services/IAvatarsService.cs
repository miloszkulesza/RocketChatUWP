using System.Threading.Tasks;
using Windows.UI.Xaml.Media;

namespace RocketChatUWP.Core.Services
{
    public interface IAvatarsService
    {
        ImageSource GetUserAvatar(string serverAddress, string username);
        ImageSource GetChannelAvatar(string serverAddress, string channelName);
    }
}
