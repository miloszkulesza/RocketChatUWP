using RocketChatUWP.Core.Models;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace RocketChatUWP.Core.Helpers
{
    public class ChannelListItemTemplateSelector : DataTemplateSelector
    {
        public DataTemplate ChannelTemplate { get; set; }
        public DataTemplate DiscussionTemplate { get; set; }
        public DataTemplate DirectConversationTemplate { get; set; }
        public DataTemplate PrivateGroupTemplate { get; set; }

        protected override DataTemplate SelectTemplateCore(object item, DependencyObject container)
        {
            if (item is Channel)
                return ChannelTemplate;
            if (item is Discussion)
                return DiscussionTemplate;
            if (item is DirectConversation)
                return DirectConversationTemplate;
            return PrivateGroupTemplate;
        }
    }
}
