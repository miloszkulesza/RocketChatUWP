using RocketChatUWP.Core.Models;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RocketChatUWP.Core.Api
{
    public interface IRocketChatRestApi
    {
        Task<bool> Login(string username, string password);
        Task<IEnumerable<Room>> GetRooms();
    }
}
