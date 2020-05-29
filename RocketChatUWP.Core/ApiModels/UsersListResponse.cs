namespace RocketChatUWP.Core.ApiModels
{
    public class UsersListResponse
    {
        public UserResponse[] users { get; set; }
        public int count { get; set; }
        public int offset { get; set; }
        public int total { get; set; }
        public bool success { get; set; }
    }

    public class UserResponse
    {
        public string _id { get; set; }
        public string type { get; set; }
        public string status { get; set; }
        public bool active { get; set; }
        public string name { get; set; }
        public string username { get; set; }
        public int utcOffset { get; set; }
    }
}
