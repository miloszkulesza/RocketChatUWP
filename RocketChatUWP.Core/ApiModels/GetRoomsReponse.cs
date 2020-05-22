using System;
using System.Collections.Generic;
using System.Text;

namespace RocketChatUWP.Core.ApiModels
{
    public class GetRoomsReponse
    {
        public IEnumerable<RoomResponse> update { get; set; }
        public bool success { get; set; }
    }

    public class RoomResponse
    {
        public string _id { get; set; }
        public string name { get; set; }
        public string t { get; set; }
        public string _updatedAt { get; set; }
        public bool Default { get; set; }
        public RoomOwner u { get; set; }
        public string[] usernames { get; set; }
        public int usersCount { get; set; }
        public string[] uids { get; set; }
        public string topic { get; set; }
        public string prid { get; set; }
        public string fname { get; set; }
    }

    public class RoomOwner
    {
        public string _id { get; set; }
        public string username { get; set; }
    }
}
