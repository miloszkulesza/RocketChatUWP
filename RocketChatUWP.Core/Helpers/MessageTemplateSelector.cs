using RocketChatUWP.Core.Models;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace RocketChatUWP.Core.Helpers
{
    public class MessageTemplateSelector : DataTemplateSelector
    {
        public DataTemplate MessageTemplate { get; set; }
        public DataTemplate UserJoinedTemplate { get; set; }
        public DataTemplate ImageMessageTemplate { get; set; }

        protected override DataTemplate SelectTemplateCore(object item, DependencyObject container)
        {
            Message message = item as Message;
            if (message.UserJoined)
                return UserJoinedTemplate;
            if (message.Attachments != null && message.Attachments.Length > 0)
                return ImageMessageTemplate;
            return MessageTemplate;
        }
    }
}
