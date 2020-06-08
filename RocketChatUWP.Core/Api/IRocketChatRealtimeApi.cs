using System.Threading.Tasks;

namespace RocketChatUWP.Core.Api
{
    public interface IRocketChatRealtimeApi
    {
        Task Connect();
        Task SendMessage(string message);
        void SetUserStatus(string status);
        void DisposeSocket();
    }
}
