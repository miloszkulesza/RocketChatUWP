using Microsoft.Toolkit.Uwp.Notifications;
using Newtonsoft.Json;
using RocketChatUWP.Core.ApiModels;
using RocketChatUWP.Core.Services;
using System;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Windows.Data.Xml.Dom;
using Windows.UI.Notifications;

namespace RocketChatUWP.Core.Api
{
    public class RocketChatRestApi : IRocketChatRestApi
    {
        private string serverUrl = "http://192.168.0.177:3000";
        private readonly IToastNotificationsService toastService;

        public RocketChatRestApi(IToastNotificationsService toastService)
        {
            this.toastService = toastService;
            //var directory = Directory.GetCurrentDirectory();
            //var path = Path.Combine(directory, "settings.json");
            //if (File.Exists(path))
            //{
            //    dynamic content = JsonConvert.DeserializeObject(File.ReadAllText(path));
            //    if (content != null)
            //        serverUrl = content.settings.serverAddress;
            //    else
            //    {
            //        var newContent = new
            //        {
            //            settings = new
            //            {
            //                serverAddress = "http://192.168.0.177:3000"
            //            }
            //        };
            //        var json = JsonConvert.SerializeObject(newContent);
            //        File.WriteAllText(path, json, Encoding.UTF8);
            //        serverUrl = newContent.settings.serverAddress;
            //    }
            //}
            //else
            //{
            //    var content = new
            //    {
            //        settings = new
            //        {
            //            serverAddress = "http://192.168.0.177:3000"
            //        }
            //    };
            //    var json = JsonConvert.SerializeObject(content);
            //    File.WriteAllText(path, json, Encoding.UTF8);
            //    serverUrl = content.settings.serverAddress;
            //}
        }

        public async Task<bool> Login(string username, string password)
        {
            using (var client = new HttpClient())
            {
                var json = new
                {
                    user = username,
                    password = password
                };
                var content = new StringContent(JsonConvert.SerializeObject(json));
                content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                HttpResponseMessage response = new HttpResponseMessage();
                try
                {
                    response = await client.PostAsync($"{serverUrl}/api/v1/login", content);
                }
                catch
                {
                    ToastVisual visual = new ToastVisual()
                    {
                        BindingGeneric = new ToastBindingGeneric()
                        {
                            Children =
                            {
                                new AdaptiveText()
                                {
                                    Text = "Błąd połączenia"
                                },

                                new AdaptiveText()
                                {
                                    Text = "Nie udało się połączyć z serwerem Rocket.Chat"
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
                    toastService.ShowToastNotification(toast);
                    throw;
                }
                var responseJson = JsonConvert.DeserializeObject<LoginResponse>(await response.Content.ReadAsStringAsync());
                if (responseJson.status == "success")
                    return true;
                else
                    return false;
            }
        }
    }
}
