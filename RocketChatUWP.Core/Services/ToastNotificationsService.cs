using Microsoft.Toolkit.Uwp.Notifications;
using System;
using Windows.Data.Xml.Dom;
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

        public void ShowErrorToastNotification(string title, string content)
        {
            ToastVisual visual = new ToastVisual()
            {
                BindingGeneric = new ToastBindingGeneric()
                {
                    Children =
                            {
                                new AdaptiveText()
                                {
                                    Text = title
                                },

                                new AdaptiveText()
                                {
                                    Text = content
                                },

                            },

                    AppLogoOverride = new ToastGenericAppLogo()
                    {
                        Source = "Assets/rocket-small-logo.png",
                        HintCrop = ToastGenericAppLogoCrop.Circle
                    }
                }
            };
            ToastContent toastContent = new ToastContent()
            {
                Visual = visual
            };

            var toastXml = new XmlDocument();
            toastXml.LoadXml(toastContent.GetContent());
            var toast = new ToastNotification(toastXml);
            ShowToastNotification(toast);
        }
    }
}
