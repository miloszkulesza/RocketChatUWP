using Microsoft.Toolkit.Uwp.Notifications;
using RocketChatUWP.Core.Models;
using System;
using Windows.Data.Xml.Dom;
using Windows.Storage;
using Windows.System;
using Windows.UI.Core;
using Windows.UI.Notifications;

namespace RocketChatUWP.Core.Services
{
    public class ToastNotificationsService : IToastNotificationsService
    {
        private string filePath;

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

        public void ShowNewMessageToastNotification(Message message, string channelType, string channelName = null)
        {
            ToastVisual visual = new ToastVisual()
            {
                BindingGeneric = new ToastBindingGeneric()
                {
                    AppLogoOverride = new ToastGenericAppLogo()
                    {
                        Source = $"{message.User.AvatarUrl}",
                        HintCrop = ToastGenericAppLogoCrop.Default
                    }
                }
            };
            switch (channelType)
            {
                case "channel":
                    visual.BindingGeneric.Children.Add(new AdaptiveText { Text = $"#{channelName}" });
                    visual.BindingGeneric.Children.Add(new AdaptiveText { Text = $"{message.User.Username}: {message.MessageContent}" });
                    break;
                case "directed":
                    visual.BindingGeneric.Children.Add(new AdaptiveText { Text = message.User.Username });
                    visual.BindingGeneric.Children.Add(new AdaptiveText { Text = message.MessageContent });
                    break;
                case "discussion":
                    visual.BindingGeneric.Children.Add(new AdaptiveText { Text = $"{channelName}" });
                    visual.BindingGeneric.Children.Add(new AdaptiveText { Text = $"{message.User.Username}: {message.MessageContent}" });
                    break;
            }
            ToastContent toastContent = new ToastContent()
            {
                Visual = visual
            };

            var toastXml = new XmlDocument();
            toastXml.LoadXml(toastContent.GetContent());
            var toast = new ToastNotification(toastXml);
            ShowToastNotification(toast);
        }

        public void ShowDownloadedFileNotification(string fileName, string filePath)
        {
            this.filePath = filePath;
            ToastVisual visual = new ToastVisual()
            {
                BindingGeneric = new ToastBindingGeneric()
                {
                    Children =
                            {
                                new AdaptiveText()
                                {
                                    Text = "Pobrano plik"
                                },

                                new AdaptiveText()
                                {
                                    Text = $"Pobrano plik {fileName}"
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
                Visual = visual,
                ActivationType = ToastActivationType.Foreground
            };

            var toastXml = new XmlDocument();
            toastXml.LoadXml(toastContent.GetContent());
            var toast = new ToastNotification(toastXml);
            toast.Activated += DownloadedToast_Activated;
            ShowToastNotification(toast);
        }

        private async void DownloadedToast_Activated(ToastNotification sender, object args)
        {

            await Windows.ApplicationModel.Core.CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, async () =>
            {
                await Launcher.LaunchFolderAsync(KnownFolders.PicturesLibrary);
            });
            
        }
    }
}
