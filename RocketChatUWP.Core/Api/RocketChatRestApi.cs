using Microsoft.Toolkit.Uwp.Notifications;
using Newtonsoft.Json;
using RocketChatUWP.Core.ApiModels;
using RocketChatUWP.Core.Models;
using RocketChatUWP.Core.Services;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Windows.Data.Xml.Dom;
using Windows.Storage;
using Windows.UI.Notifications;

namespace RocketChatUWP.Core.Api
{
    public class RocketChatRestApi : IRocketChatRestApi
    {
        private string serverAddress;
        private readonly IToastNotificationsService toastService;

        public User User { get; set; }

        public RocketChatRestApi(IToastNotificationsService toastService)
        {
            this.toastService = toastService;
            SetServerAddress();  
        }

        private async void SetServerAddress()
        {
            StorageFolder storageFolder = ApplicationData.Current.LocalFolder;
            StorageFile file = await storageFolder.CreateFileAsync("settings.json", CreationCollisionOption.OpenIfExists);
            if (file.IsAvailable)
            {
                dynamic content = JsonConvert.DeserializeObject(await FileIO.ReadTextAsync(file));
                if (content != null)
                {
                    if(content.settings.restServerAddress != null)
                        serverAddress = content.settings.restServerAddress;
                    else
                    {
                        var newContent = new
                        {
                            settings = new
                            {
                                restServerAddress = "http://localhost:3000"
                            }
                        };
                        var json = JsonConvert.SerializeObject(newContent);
                        await FileIO.WriteTextAsync(file, json);
                        serverAddress = newContent.settings.restServerAddress;
                    }
                }
                else
                {
                    var newContent = new
                    {
                        settings = new
                        {
                            restServerAddress = "http://localhost:3000"
                        }
                    };
                    var json = JsonConvert.SerializeObject(newContent);
                    await FileIO.WriteTextAsync(file, json);
                    serverAddress = newContent.settings.restServerAddress;
                }
            }
            else
            {
                var content = new
                {
                    settings = new
                    {
                        restServerAddress = "http://localhost:3000"
                    }
                };
                var json = JsonConvert.SerializeObject(content);
                await FileIO.WriteTextAsync(file, json);
                serverAddress = content.settings.restServerAddress;
            }
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
                    response = await client.PostAsync($"{serverAddress}/api/v1/login", content);
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
                {
                    User = new User(responseJson);
                    return true;
                }
                else
                    return false;
            }
        }

        public async Task<IEnumerable<Room>> GetRooms()
        {
            using(var client = new HttpClient())
            {
                var request = new HttpRequestMessage();
                request.Headers.Add("X-Auth-Token", User.AuthToken);
                request.Headers.Add("X-User-Id", User.Id);
                request.RequestUri = new Uri($"{serverAddress}/api/v1/rooms.get");
                request.Method = HttpMethod.Get;
                var response = await client.SendAsync(request);
                var responseContent = await response.Content.ReadAsStringAsync();
                var responseJson = JsonConvert.DeserializeObject<GetRoomsReponse>(responseContent);
                var rooms = new List<Room>();
                foreach(var room in responseJson.update)
                {
                    rooms.Add(new Room(room));
                }
                return rooms;
            }
        }
    }
}
