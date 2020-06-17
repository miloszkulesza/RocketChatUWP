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
        User User { get; set; }
        Task<bool> Login(string username, string password);
        Task<IEnumerable<Room>> GetRooms();
        void SetUserStatus(string message = null);
        Task<IEnumerable<Message>> GetChannelHistory(string roomId, int offset = 0, int count = 20);
        Task<IEnumerable<User>> GetUsersList();
        void Logout();
        Task<IEnumerable<Message>> GetDirectMessages(string roomId);
        void PostChatMessage(string roomId, string message);
        Task<BitmapImage> GetImage(string imageUrl);
        Task<MemoryStream> GetFile(string fileUrl);
    }
}
