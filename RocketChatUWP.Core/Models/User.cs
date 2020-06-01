using Prism.Mvvm;
using RocketChatUWP.Core.ApiModels;
using Windows.UI.Xaml.Media;

namespace RocketChatUWP.Core.Models
{
    public class User : BindableBase
    {
        public User()
        {

        }

        public User(LoginResponse loginResponse)
        {
            AuthToken = loginResponse.data.authToken;
            Id = loginResponse.data.me._id;
            Name = loginResponse.data.me.name;
            Active = loginResponse.data.me.active;
            Username = loginResponse.data.me.username;
            Roles = loginResponse.data.me.roles;
            AvatarUrl = loginResponse.data.me.avatarUrl;
            UserPresence = new UserPresence
            {
                Status = loginResponse.data.me.status,
                UtcOffset = loginResponse.data.me.utcOffset
            };
            if (loginResponse.data.me.statusText != null)
                UserPresence.StatusText = loginResponse.data.me.statusText;
            else
                UserPresence.StatusText = loginResponse.data.me.status;
        }

        public User(UserResponse response)
        {
            Id = response._id;
            Name = response.name;
            Active = response.active;
            Username = response.username;
            UserPresence = new UserPresence
            {
                Status = response.status,
                StatusText = response.statusText,
                UtcOffset = response.utcOffset
            };
        }

        private string authToken;
        public string AuthToken
        {
            get { return authToken; }
            set { SetProperty(ref authToken, value); }
        }

        private string id;
        public string Id
        {
            get { return id; }
            set { SetProperty(ref id, value); }
        }

        private string name;
        public string Name
        {
            get { return name; }
            set { SetProperty(ref name, value); }
        }

        private bool active;
        public bool Active
        {
            get { return active; }
            set { SetProperty(ref active, value); }
        }

        private string username;
        public string Username
        {
            get { return username; }
            set { SetProperty(ref username, value); }
        }

        private string[] roles;
        public string[] Roles
        {
            get { return roles; }
            set { SetProperty(ref roles, value); }
        }

        private string avatarUrl;
        public string AvatarUrl
        {
            get { return avatarUrl; }
            set { SetProperty(ref avatarUrl, value); }
        }

        private ImageSource avatar;
        public ImageSource Avatar
        {
            get { return avatar; }
            set { SetProperty(ref avatar, value); }
        }

        private UserPresence userPresence;
        public UserPresence UserPresence
        {
            get { return userPresence; }
            set { SetProperty(ref userPresence, value); }
        }
    }

    public class UserPresence : BindableBase
    {
        private string status;
        public string Status
        {
            get { return status; }
            set { SetProperty(ref status, value); }
        }

        private int? utcOffset;
        public int? UtcOffset
        {
            get { return utcOffset; }
            set { SetProperty(ref utcOffset, value); }
        }

        private string statusText;
        public string StatusText
        {
            get { return statusText; }
            set { SetProperty(ref statusText, value); }
        }
    }
}
