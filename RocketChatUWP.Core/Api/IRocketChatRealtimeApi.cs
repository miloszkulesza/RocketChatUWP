using System.Threading.Tasks;

namespace RocketChatUWP.Core.Api
{
    public interface IRocketChatRealtimeApi
    {
        string ServerAddress { get; set; }
        bool IsConnected { get; set; }
        Task Connect();
        Task SendMessage(string message);
        Task SetUserStatus(string status);
        void DisposeSocket();
    }
}
