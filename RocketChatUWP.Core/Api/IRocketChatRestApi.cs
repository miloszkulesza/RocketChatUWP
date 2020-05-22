using System.Threading.Tasks;

namespace RocketChatUWP.Core.Api
{
    public interface IRocketChatRestApi
    {
        Task<bool> Login(string username, string password);
    }
}
