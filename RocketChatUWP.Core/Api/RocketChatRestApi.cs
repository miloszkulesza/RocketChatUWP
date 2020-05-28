using Microsoft.Toolkit.Uwp.Notifications;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RocketChatUWP.Core.ApiModels;
using RocketChatUWP.Core.Models;
using RocketChatUWP.Core.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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
        private readonly IAvatarsService avatarsService;

        public User User { get; set; }

        public RocketChatRestApi(
            IToastNotificationsService toastService,
            IAvatarsService avatarsService)
        {
            this.toastService = toastService;
            this.avatarsService = avatarsService;
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
                    if(content.settings != null)
                    {
                        if(content.settings.restServerAddress != null)
                            serverAddress = content.settings.restServerAddress;
                        else
                        {
                            content.settings.restServerAddress = "http://localhost:3000";
                            var json = JsonConvert.SerializeObject(content);
                            FileIO.WriteTextAsync(file, json);
                            serverAddress = content.settings.restServerAddress;
                        }
                    }
                    else
                    {
                        content.settings = new
                        {
                            restServerAddress = "http://localhost:3000"
                        };
                        var json = JsonConvert.SerializeObject(content);
                        FileIO.WriteTextAsync(file, json);
                        serverAddress = content.settings.restServerAddress;
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
                    FileIO.WriteTextAsync(file, json);
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
                FileIO.WriteTextAsync(file, json);
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
                    toastService.ShowErrorToastNotification("Błąd połączenia", "Nie udało się połączyć z serwerem Rocket.Chat.");
                    throw;
                }
                var responseString = await response.Content.ReadAsStringAsync();
                var responseJson = JsonConvert.DeserializeObject<LoginResponse>(responseString);
                if (responseJson.status == "success")
                {
                    User = new User(responseJson);
                    var avatar = await avatarsService.GetAvatar(User.AvatarUrl);
                    User.Avatar = avatar;
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

        public async void SetUserStatus(string message = null)
        {
            using (var client = new HttpClient())
            {
                var request = new HttpRequestMessage();
                request.Headers.Add("X-Auth-Token", User.AuthToken);
                request.Headers.Add("X-User-Id", User.Id);
                request.RequestUri = new Uri($"{serverAddress}/api/v1/users.setStatus");
                request.Method = HttpMethod.Post;
                JObject contentJson = new JObject();
                if (message == null)
                    contentJson.Add("message", "");
                else
                    contentJson.Add("message", message);
                var json = JsonConvert.SerializeObject(contentJson);
                var content = new StringContent(json);
                content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                request.Content = content;
                var response = await client.SendAsync(request);
                await response.Content.ReadAsStringAsync();
            }
        }

        public async Task<IEnumerable<Message>> GetChannelHistory(string roomId, int offset = 0, int count = 20)
        {
            using(var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Add("X-Auth-Token", User.AuthToken);
                client.DefaultRequestHeaders.Add("X-User-Id", User.Id);
                var response = await client.GetAsync($"{serverAddress}/api/v1/channels.history?roomId={roomId}&offset={offset}&count={count}");
                var responseContent = JsonConvert.DeserializeObject<ChatHistoryResponse>(await response.Content.ReadAsStringAsync());
                List<Message> messages = new List<Message>();
                for (int i = 0; i < responseContent.messages.Length; i++)
                {
                    messages.Insert(0, new Message(responseContent.messages[i]));
                }
                return messages;
            }
        }
    }
}
