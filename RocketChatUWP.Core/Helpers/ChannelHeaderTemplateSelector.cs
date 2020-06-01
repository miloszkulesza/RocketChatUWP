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

        protected override DataTemplate SelectTemplateCore(object item, DependencyObject container)
        {
            if (item != null)
            {
                Room room = item as Room;
                if (room is Discussion)
                    return DiscussionHeaderTemplate;
                if (room is DirectConversation)
                    return PrivateConversationHeaderTemplate;
                if (room is Channel)
                    return ChannelHeaderTemplate;
            }
            return EmptyHeaderTemplate;
        }
    }
}
