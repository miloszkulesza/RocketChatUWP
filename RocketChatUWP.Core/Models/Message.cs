using Prism.Mvvm;
using RocketChatUWP.Core.ApiModels;
using System;

namespace RocketChatUWP.Core.Models
{
    public class Message : BindableBase
    {
        public Message(ChatHistoryMessage message)
        {
            Id = message._id;
            RoomId = message.rid;
            MessageContent = message.msg;
            Timestamp = Convert.ToDateTime(message.ts);
            User = new User { Id = message.u._id, Username = message.u.username };
            Groupable = message.groupable;
            UpdatedAt = Convert.ToDateTime(message._updatedAt);
        }

        private string id;
        public string Id
        {
            get { return id; }
            set { SetProperty(ref id, value); }
        }

        private string roomId;
        public string RoomId
        {
            get { return roomId; }
            set { SetProperty(ref roomId, value); }
        }

        private string messageContent;
        public string MessageContent
        {
            get { return messageContent; }
            set { SetProperty(ref messageContent, value); }
        }

        private DateTime timestamp;
        public DateTime Timestamp
        {
            get { return timestamp; }
            set { SetProperty(ref timestamp, value); }
        }

        private User user;
        public User User
        {
            get { return user; }
            set { SetProperty(ref user, value); }
        }

        private bool groupable;
        public bool Groupable
        {
            get { return groupable; }
            set { SetProperty(ref groupable, value); }
        }

        private DateTime updatedAt;
        public DateTime UpdatedAt
        {
            get { return updatedAt; }
            set { SetProperty(ref updatedAt, value); }
        }
    }
}
