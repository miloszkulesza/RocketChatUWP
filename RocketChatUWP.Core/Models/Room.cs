using RocketChatUWP.Core.ApiModels;
using System;
using Windows.UI.Xaml.Media;

namespace RocketChatUWP.Core.Models
{
    public class Room
    {
        public Room(RoomResponse roomResponse)
        {
            Id = roomResponse._id;
            Name = roomResponse.name;
            T = roomResponse.t;
            UpdatedAt = Convert.ToDateTime(roomResponse._updatedAt);
            Default = roomResponse.Default;
            RoomOwner = roomResponse.u;
            Usernames = roomResponse.usernames;
            UsersCount = roomResponse.usersCount;
            UserIds = roomResponse.uids;
            Topic = roomResponse.topic;
            ParentId = roomResponse.prid;
            DiscussionName = roomResponse.fname;
        }

        public string Id { get; set; }
        public string Name { get; set; }
        public string T { get; set; }
        public DateTime UpdatedAt { get; set; }
        public bool Default { get; set; }
        public RoomOwner RoomOwner { get; set; }
        public string[] Usernames { get; set; }
        public int UsersCount { get; set; }
        public string[] UserIds { get; set; }
        public string Topic { get; set; }
        public string ParentId { get; set; }
        public string DiscussionName { get; set; }
        public ImageSource Avatar { get; set; }
        public bool IsChannel { get; set; } = false;
        public bool IsDiscussion { get; set; } = false;
        public bool IsPrivateGroup { get; set; } = false;
        public bool IsPrivateConversation { get; set; } = false;
    }
}
