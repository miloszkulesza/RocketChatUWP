using Microsoft.Toolkit.Uwp.Notifications;
using RocketChatUWP.Core.Api;
using RocketChatUWP.Core.Models;
using System;
using Windows.Data.Xml.Dom;
using Windows.Storage;
using Windows.System;
using Windows.UI.Core;
using Windows.UI.Notifications;
using Windows.UI.Xaml.Media.Imaging;

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

        public void ShowNewMessageToastNotification(Message message, Room channel, bool isImage = false)
        {
            ToastVisual visual = new ToastVisual()
            {
                BindingGeneric = new ToastBindingGeneric()
                {
                    AppLogoOverride = new ToastGenericAppLogo()
                    {
                        HintCrop = ToastGenericAppLogoCrop.Default
                    }
                }
            };
            if (channel is Channel)
            {
                visual.BindingGeneric.AppLogoOverride.Source = channel.AvatarUrl;
                visual.BindingGeneric.Children.Add(new AdaptiveText { Text = $"#{channel.Name}" });
                if (isImage)
                    visual.BindingGeneric.Children.Add(new AdaptiveText { Text = $"{message.User.Username} przesłał zdjęcie" });
                else
                    visual.BindingGeneric.Children.Add(new AdaptiveText { Text = $"{message.User.Username}: {message.MessageContent}" });
            }
            if (channel is DirectConversation)
            {
                visual.BindingGeneric.AppLogoOverride.Source = message.User.AvatarUrl;
                visual.BindingGeneric.Children.Add(new AdaptiveText { Text = message.User.Username });
                if (isImage)
                    visual.BindingGeneric.Children.Add(new AdaptiveText { Text = $"Przesłano zdjęcie" });
                else
                    visual.BindingGeneric.Children.Add(new AdaptiveText { Text = message.MessageContent });
            }
            if (channel is Discussion)
            {
                visual.BindingGeneric.AppLogoOverride.Source = channel.AvatarUrl;
                visual.BindingGeneric.Children.Add(new AdaptiveText { Text = $"{channel.Name}" });
                if (isImage)
                    visual.BindingGeneric.Children.Add(new AdaptiveText { Text = $"{message.User.Username} przesłał zdjęcie" });
                else
                    visual.BindingGeneric.Children.Add(new AdaptiveText { Text = $"{message.User.Username}: {message.MessageContent}" });
            }
            ToastContent toastContent = new ToastContent()
            {
                Visual = visual,
                DisplayTimestamp = message.Timestamp
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
                                }
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
                DisplayTimestamp = DateTime.Now,
                Actions = new ToastActionsCustom()
                {
                    Buttons =
                    {
                        new ToastButton("Otwórz folder", "openDownloadsFolder")
                        {
                            ActivationType = ToastActivationType.Foreground
                        }
                    }
                }
            };
            var toastXml = new XmlDocument();
            toastXml.LoadXml(toastContent.GetContent());
            var toast = new ToastNotification(toastXml);
            toast.Activated += DownloadedToast_Activated;
            ShowToastNotification(toast);
        }

        private async void DownloadedToast_Activated(ToastNotification sender, object args)
        {
            if ((args as ToastActivatedEventArgs).Arguments == "openDownloadsFolder")
            {
                await Windows.ApplicationModel.Core.CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, async () =>
                {
                    await Launcher.LaunchFolderAsync(KnownFolders.PicturesLibrary);
                });
            }
        }
    }
}
