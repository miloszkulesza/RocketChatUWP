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
            if (message.t == "uj")
                UserJoined = true;
            else
                UserJoined = false;
        }

        public Message(NewMessageNotification message)
        {
            Id = message.fields.args[0]._id;
            RoomId = message.fields.args[0].rid;
            MessageContent = message.fields.args[0].msg;
            Timestamp = new DateTime(1970, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Utc);
            double seconds = double.Parse(message.fields.args[0].ts.date);
            Timestamp =  Timestamp.AddMilliseconds(seconds).ToLocalTime();
            User = new User { Id = message.fields.args[0].u._id, Username = message.fields.args[0].u.username };
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

        private bool userJoined;
        public bool UserJoined
        {
            get { return userJoined; }
            set { SetProperty(ref userJoined, value); }
        }
    }
}
