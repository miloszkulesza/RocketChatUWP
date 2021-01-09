using RocketChatUWP.Core.Models;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media.Imaging;

namespace RocketChatUWP.Core.Api
{
    public interface IRocketChatRestApi
    {
        string ServerAddress { get; set; }
        User User { get; set; }
        Task<bool> Login(string username, string password);
        Task<IEnumerable<Room>> GetRooms();
        Task SetUserStatus(string message = null);
        Task<IEnumerable<Message>> GetChannelHistory(string roomId, int offset = 0, int count = 20);
        Task<IEnumerable<User>> GetUsersList();
        Task Logout();
        Task<IEnumerable<Message>> GetDirectMessages(string roomId, int offset = 0, int count = 20);
        Task PostChatMessage(string roomId, string message);
        Task<BitmapImage> GetImage(string imageUrl);
        Task<MemoryStream> GetFile(string fileUrl);
        Task<IEnumerable<Message>> GetPrivateGroupHistory(string roomId, int offset = 0, int count = 20);
    }
}
