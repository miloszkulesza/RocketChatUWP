using RocketChatUWP.Core.Models;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace RocketChatUWP.Core.Helpers
{
    public class ChannelHeaderTemplateSelector : DataTemplateSelector
    {
        public DataTemplate ChannelHeaderTemplate { get; set; }
        public DataTemplate DiscussionHeaderTemplate { get; set; }
        public DataTemplate PrivateConversationHeaderTemplate { get; set; }
        public DataTemplate EmptyHeaderTemplate { get; set; }
        public DataTemplate PrivateGroupHeaderTemplate { get; set; }

        protected override DataTemplate SelectTemplateCore(object item, DependencyObject container)
        {
            if (item != null)
            {
                Room room = item as Room;
                switch (room.GetType().Name)
                {
                    case "Channel":
                        return ChannelHeaderTemplate;

                    case "Discussion":
                        return DiscussionHeaderTemplate;

                    case "DirectConversation":
                        return PrivateConversationHeaderTemplate;

                    case "PrivateGroup":
                        return PrivateGroupHeaderTemplate;

                    default:
                        return EmptyHeaderTemplate;
                }
            }
            return EmptyHeaderTemplate;
        }
    }
}
