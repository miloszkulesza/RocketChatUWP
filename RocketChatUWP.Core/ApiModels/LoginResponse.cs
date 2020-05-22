namespace RocketChatUWP.Core.ApiModels
{
    public class LoginResponse
    {
        public string status { get; set; }
        public LoginResponseData data { get; set; }
    }

    public class LoginResponseData
    {
        public string authToken { get; set; }
        public string userId { get; set; }
        public LoginResponseDataMe me { get; set; }
    }

    public class LoginResponseDataMe
    {
        public string _id { get; set; }
        public string name { get; set; }
        public LoginResponseEmails[] emails { get; set; }
        public string status { get; set; }
        public string statusConnection { get; set; }
        public string username { get; set; }
        public int utcOffset { get; set; }
        public bool active { get; set; }
        public string[] roles { get; set; }
        public dynamic settings { get; set; }
    }

    public class LoginResponseEmails
    {
        public string address { get; set; }
        public bool verified { get; set; }
    }
}
