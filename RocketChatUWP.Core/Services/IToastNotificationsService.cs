using RocketChatUWP.Core.Models;
using Windows.UI.Notifications;

namespace RocketChatUWP.Core.Services
{
    public interface IToastNotificationsService
    {
        void ShowToastNotification(ToastNotification toastNotification);
        void ShowErrorToastNotification(string title, string content);
        void ShowNewMessageToastNotification(Message message, string channelType, string channelName = null);
    }
}
