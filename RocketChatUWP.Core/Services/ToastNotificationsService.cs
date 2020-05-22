using System;

using Windows.UI.Notifications;

namespace RocketChatUWP.Core.Services
{
    public class ToastNotificationsService : IToastNotificationsService
    {
        public void ShowToastNotification(ToastNotification toastNotification)
        {
            try
            {
                ToastNotificationManager.CreateToastNotifier().Show(toastNotification);
            }
            catch (Exception)
            {
                // TODO WTS: Adding ToastNotification can fail in rare conditions, please handle exceptions as appropriate to your scenario.
            }
        }
    }
}
