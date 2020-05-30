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
                if (room.IsDiscussion)
                    return DiscussionHeaderTemplate;
                if (room.IsPrivateConversation)
                    return PrivateConversationHeaderTemplate;
                if (room.IsChannel)
                    return ChannelHeaderTemplate;
            }
            return EmptyHeaderTemplate;
        }
    }
}
