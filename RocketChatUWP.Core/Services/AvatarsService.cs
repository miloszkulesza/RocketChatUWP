﻿using System;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;

namespace RocketChatUWP.Core.Services
{
    public class AvatarsService : IAvatarsService
    {
        public ImageSource GetUserAvatar(string serverAddress, string username)
        {
            var imageAddress = $"{serverAddress}/avatar/{username}?format=png";
            var img = new BitmapImage(new Uri(imageAddress));
            return img;
        }

        public ImageSource GetChannelAvatar(string serverAddress, string channelName)
        {
            var imageAddress = $"{serverAddress}/avatar/@{channelName}?format=png";
            var img = new BitmapImage(new Uri(imageAddress));
            return img;
        }

        public string GetUserAvatarUrl(string serverAddress, string username)
        {
            return $"{serverAddress}/avatar/{username}?format=png";
        }

        public string GetChannelAvatarUrl(string serverAddress, string channelName)
        {
            return $"{serverAddress}/avatar/@{channelName}?format=png";
        }
    }
}
