namespace RocketChatUWP.Core.Api
{
    public interface IRocketChatRealtimeApi
    {
        void Connect();
        void SendMessage(string message);
        void SetUserStatus(string status);
    }
}
