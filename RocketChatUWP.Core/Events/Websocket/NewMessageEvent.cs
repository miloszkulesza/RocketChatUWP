using Prism.Events;
using RocketChatUWP.Core.ApiModels;

namespace RocketChatUWP.Core.Events.Websocket
{
    public class NewMessageEvent : PubSubEvent<NewMessageNotification>
    {
    }
}
