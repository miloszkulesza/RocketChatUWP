using RocketChatUWP.Core.ApiModels;
using System.Collections.ObjectModel;

namespace RocketChatUWP.Core.Models
{
    public class DirectConversation : Room
    {
        public string[] Usernames { get; set; }
        public string[] UserIds { get; set; }
        public User User { get; set; }

        public DirectConversation(RoomResponse room): base(room)
        {
            Usernames = room.usernames;
            UserIds = room.uids;
        }
    }
}
