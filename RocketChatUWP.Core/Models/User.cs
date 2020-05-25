using RocketChatUWP.Core.ApiModels;
using Windows.UI.Xaml.Media;

namespace RocketChatUWP.Core.Models
{
    public class User
    {
        public User(LoginResponse loginResponse)
        {
            AuthToken = loginResponse.data.authToken;
            Id = loginResponse.data.me._id;
            Name = loginResponse.data.me.name;
            Status = loginResponse.data.me.status;
            Active = loginResponse.data.me.active;
            Username = loginResponse.data.me.username;
            UtcOffset = loginResponse.data.me.utcOffset;
            Roles = loginResponse.data.me.roles;
            AvatarUrl = loginResponse.data.me.avatarUrl;
        }

        public string AuthToken { get; set; }
        public string Id { get; set; }
        public string Name { get; set; }
        public string Status { get; set; }
        public bool Active { get; set; }
        public string Username { get; set; }
        public int UtcOffset { get; set; }
        public string[] Roles { get; set; }
        public string AvatarUrl { get; set; }
        public ImageSource Avatar { get; set; }
    }
}
