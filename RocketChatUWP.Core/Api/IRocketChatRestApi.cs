using RocketChatUWP.Core.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

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
    }
}
