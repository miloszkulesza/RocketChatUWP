using System.Threading.Tasks;
using Windows.UI.Xaml.Media;

namespace RocketChatUWP.Core.Services
{
    public interface IAvatarsService
    {
        ImageSource GetAvatar(string serverAddress, string username);
    }
}
