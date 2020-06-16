using Prism.Mvvm;
using RocketChatUWP.Core.ApiModels;
using System;
using System.IO;
using System.Threading.Tasks;
using Windows.Storage.Streams;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Imaging;

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
            if (message.file != null)
                File = new MessageFile(message.file);
            if(message.attachments != null)
            {
                Attachments = new Attachment[message.attachments.Length];
                for (int i = 0; i < Attachments.Length; i++)
                {
                    Attachments[i] = new Attachment(message.attachments[i]);
                }
            }
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

        private MessageFile file;
        public MessageFile File
        {
            get { return file; }
            set { SetProperty(ref file, value); }
        }

        private Attachment[] attachments;
        public Attachment[] Attachments
        {
            get { return attachments; }
            set { SetProperty(ref attachments, value); }
        }
    }

    public class MessageFile
    {
        public MessageFile(MessageFileInfo file)
        {
            Id = file._id;
            Name = file.name;
            Type = file.type;
        }

        public string Id { get; set; }
        public string Name { get; set; }
        public string Type { get; set; }
    }

    public class Attachment
    {
        public Attachment(MessageAttachment attachment)
        {
            ImageSize = attachment.image_size;
            ImageUrl = attachment.image_url;
            ImageHeight = attachment.image_dimensions.height;
            ImageWidth = attachment.image_dimensions.width;
            ImagePreview = Base64ToBitmapImage(attachment.image_preview).Result;
            Description = attachment.description;
        }
        public int ImageSize { get; set; }
        public string ImageUrl { get; set; }
        public int ImageWidth { get; set; }
        public int ImageHeight { get; set; }
        public BitmapImage ImagePreview { get; set; }
        public string Description { get; set; }

        private async Task<BitmapImage> Base64ToBitmapImage(string source)
        {
            var ims = new InMemoryRandomAccessStream();
            var bytes = Convert.FromBase64String(source);
            var dataWriter = new DataWriter(ims);
            dataWriter.WriteBytes(bytes);
            await dataWriter.StoreAsync();
            ims.Seek(0);
            var img = new BitmapImage();
            img.SetSource(ims);
            return img;
        }
    }
}
