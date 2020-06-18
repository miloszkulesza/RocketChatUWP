using RocketChatUWP.Core.Models;
using Windows.UI.Notifications;

namespace RocketChatUWP.Core.Services
{
    public interface IToastNotificationsService
    {
        void ShowToastNotification(ToastNotification toastNotification);
        void ShowErrorToastNotification(string title, string content);
        void ShowNewMessageToastNotification(Message message, Room channel, bool isImage = false);
        void ShowDownloadedFileNotification(string fileName, string filePath);
    }
}
