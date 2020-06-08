using Prism.Mvvm;
using RocketChatUWP.Core.ApiModels;
using System;
using Windows.UI.Text;
using Windows.UI.Xaml.Media;

namespace RocketChatUWP.Core.Models
{
    public abstract class Room : BindableBase
    {
        public Room()
        {
            
        }

        public Room(RoomResponse roomResponse)
        {
            Id = roomResponse._id;
            Name = roomResponse.name;
            T = roomResponse.t;
            UpdatedAt = Convert.ToDateTime(roomResponse._updatedAt);
            Default = roomResponse.Default;
            RoomOwner = roomResponse.u;
            UsersCount = roomResponse.usersCount;
            HasUnreadedMessages = false;
            ChannelFontWeight = FontWeights.Normal;
        }

        public string Id { get; set; }
        public string Name { get; set; }
        public string T { get; set; }
        public DateTime UpdatedAt { get; set; }
        public bool Default { get; set; }
        public RoomOwner RoomOwner { get; set; }
        public int UsersCount { get; set; }
        public ImageSource Avatar { get; set; }

        private bool hasUnreadedMessages;
        public bool HasUnreadedMessages
        {
            get
            {
                return hasUnreadedMessages;
            }
            set
            {
                SetProperty(ref hasUnreadedMessages, value);
            }
        }

        private FontWeight channelFontWeight;
        public FontWeight ChannelFontWeight
        {
            get { return channelFontWeight; }
            set { SetProperty(ref channelFontWeight, value); }
        }
    }
}
