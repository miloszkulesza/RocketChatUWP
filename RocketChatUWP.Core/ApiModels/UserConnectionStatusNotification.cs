namespace RocketChatUWP.Core.ApiModels
{
    public class UserConnectionStatusNotification
    {
        public string msg { get; set; }
        public string collection { get; set; }
        public string id { get; set; }
        public UserConnectionStatusFields fields { get; set; }
    }

    public class UserConnectionStatusFields
    {
        public string eventName { get; set; }
        public object[] args { get; set; }
        public string userId { get; set; }
        public string status { get; set; }
        public string username { get; set; }
        public string message { get; set; }
    }
}
