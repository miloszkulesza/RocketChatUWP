using System;
using System.Collections.Generic;
using System.Text;

namespace RocketChatUWP.Core.ApiModels
{
    public class UsersPresenceResponse
    {
        public UserPresenceResponse[] users { get; set; }
    }

    public class UserPresenceResponse
    {
        public string _id { get; set; }
        public string name { get; set; }
        public string username { get; set; }
        public string status { get; set; }
        public int? utcOffset { get; set; }
        public string statusText { get; set; }
    }
}
