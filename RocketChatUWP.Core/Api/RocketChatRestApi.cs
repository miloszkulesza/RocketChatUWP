using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Prism.Events;
using RocketChatUWP.Core.ApiModels;
using RocketChatUWP.Core.Events.Login;
using RocketChatUWP.Core.Helpers;
using RocketChatUWP.Core.Models;
using RocketChatUWP.Core.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Windows.UI.Xaml.Media.Imaging;

namespace RocketChatUWP.Core.Api
{
    public class RocketChatRestApi : IRocketChatRestApi
    {
        #region private members
        private readonly IToastNotificationsService toastService;
        private readonly IAvatarsService avatarsService;
        private readonly IEventAggregator eventAggregator;
        private readonly HttpClient httpClient = new HttpClient();
        #endregion

        #region properties
        public User User { get; set; }
        public string ServerAddress { get; set; }
        #endregion

        #region ctor
        public RocketChatRestApi(
            IToastNotificationsService toastService,
            IAvatarsService avatarsService,
            IEventAggregator eventAggregator)
        {
            this.toastService = toastService;
            this.avatarsService = avatarsService;
            this.eventAggregator = eventAggregator;
            this.eventAggregator.GetEvent<ServerAddressChangedEvent>().Subscribe(ServerAddressChangedHandler);
            GetServerAddress();
        }
        #endregion

        #region private methods
        private async Task GetServerAddress()
        {
            var address = await ServerAddressHelper.GetServerAddress();
            ServerAddress = address.HttpAddress;
        }

        private HttpRequestMessage CreateAuthorizedRequest(HttpMethod method, Uri uri)
        {
            var request = new HttpRequestMessage();
            request.Headers.Add("X-Auth-Token", User.AuthToken);
            request.Headers.Add("X-User-Id", User.Id);
            request.Method = method;
            request.RequestUri = uri;
            return request;
        }
        #endregion

        #region public methods
        public async Task<bool> Login(string username, string password)
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
                response = await httpClient.PostAsync($"{ServerAddress}/api/v1/login", content);
            }
            catch
            {
                toastService.ShowErrorToastNotification("Błąd połączenia", "Nie udało się połączyć z serwerem http Rocket.Chat.");
                throw;
            }
            var responseString = await response.Content.ReadAsStringAsync();
            var responseJson = JsonConvert.DeserializeObject<LoginResponse>(responseString);
            if (responseJson.status == "success")
            {
                User = new User(responseJson);
                var avatar = avatarsService.GetUserAvatar(ServerAddress, User.Username);
                User.Avatar = avatar;
                return true;
            }
            else
                return false;
        }

        public async Task<IEnumerable<Room>> GetRooms()
        {
            var request = CreateAuthorizedRequest(HttpMethod.Get, new Uri($"{ServerAddress}/api/v1/rooms.get"));
            var response = await httpClient.SendAsync(request);
            var responseContent = await response.Content.ReadAsStringAsync();
            var responseJson = JsonConvert.DeserializeObject<GetRoomsReponse>(responseContent);
            var rooms = new List<Room>();
            foreach(var room in responseJson.update)
            {
                Room newRoom;
                if (room.t == "c" && room.prid == null)
                {
                    newRoom = new Channel(room);
                    newRoom.Avatar = avatarsService.GetChannelAvatar(ServerAddress, newRoom.Name);
                    newRoom.AvatarUrl = avatarsService.GetChannelAvatarUrl(ServerAddress, newRoom.Name);
                }
                else if (room.t == "c" && room.prid != null)
                {
                    newRoom = new Discussion(room);
                    newRoom.Avatar = avatarsService.GetChannelAvatar(ServerAddress, newRoom.Name);
                    newRoom.AvatarUrl = avatarsService.GetChannelAvatarUrl(ServerAddress, newRoom.Name);
                }
                else if (room.t == "d")
                    newRoom = new DirectConversation(room);
                else
                {
                    newRoom = new PrivateGroup(room);
                    newRoom.Avatar = avatarsService.GetChannelAvatar(ServerAddress, newRoom.Name);
                    newRoom.AvatarUrl = avatarsService.GetChannelAvatarUrl(ServerAddress, newRoom.Name);
                }
                rooms.Add(newRoom);
            }
            return rooms;
        }

        public async Task SetUserStatus(string message = null)
        {
            var request = CreateAuthorizedRequest(HttpMethod.Post, new Uri($"{ServerAddress}/api/v1/users.setStatus"));
            JObject contentJson = new JObject();
            if (message == null)
                contentJson.Add("message", "");
            else
                contentJson.Add("message", message);
            var json = JsonConvert.SerializeObject(contentJson);
            var content = new StringContent(json);
            content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            request.Content = content;
            var response = await httpClient.SendAsync(request);
            await response.Content.ReadAsStringAsync();
        }

        public async Task<IEnumerable<Message>> GetChannelHistory(string roomId, int offset = 0, int count = 20)
        {
            var request = CreateAuthorizedRequest(HttpMethod.Get, new Uri($"{ServerAddress}/api/v1/channels.history?roomId={roomId}&offset={offset}&count={count}"));
            var response = await httpClient.SendAsync(request);
            var responseContent = JsonConvert.DeserializeObject<ChatHistoryResponse>(await response.Content.ReadAsStringAsync());
            List<Message> messages = new List<Message>();
            if (!responseContent.success)
            {
                toastService.ShowErrorToastNotification("Wczytywanie kanału", "Nie udało się pobrać wiadomości z kanału");
                return messages;
            }
            for (int i = 0; i < responseContent.messages.Length; i++)
            {
                messages.Insert(0, new Message(responseContent.messages[i]));
            }
            return messages;
        }

        public async Task<IEnumerable<User>> GetUsersList()
        {
            var request = CreateAuthorizedRequest(HttpMethod.Get, new Uri($"{ServerAddress}/api/v1/users.list"));
            var response = await httpClient.SendAsync(request);
            var responseContent = JsonConvert.DeserializeObject<UsersListResponse>(await response.Content.ReadAsStringAsync());
            var users = new List<User>();
            foreach (var user in responseContent.users)
            {
                User newUser = new User(user);
                newUser.AvatarUrl = avatarsService.GetUserAvatarUrl(ServerAddress, newUser.Username);
                newUser.Avatar = avatarsService.GetUserAvatar(ServerAddress, newUser.Username);
                users.Add(newUser);
            }
            return users;
        }

        public async Task Logout()
        {
            var request = CreateAuthorizedRequest(HttpMethod.Get, new Uri($"{ServerAddress}/api/v1/logout"));
            var response = await httpClient.SendAsync(request);

            if(response.IsSuccessStatusCode)
                User = null;
        }

        public async Task<IEnumerable<Message>> GetDirectMessages(string roomId, int offset = 0, int count = 20)
        {
            var request = CreateAuthorizedRequest(HttpMethod.Get, new Uri($"{ServerAddress}/api/v1/im.history?roomId={roomId}&offset={offset}&count={count}"));
            var response = await httpClient.SendAsync(request);
            var responseContent = JsonConvert.DeserializeObject<ChatHistoryResponse>(await response.Content.ReadAsStringAsync());
            var messages = new List<Message>();
            foreach(var message in responseContent.messages)
            {
                messages.Insert(0, new Message(message));
            }
            return messages;
        }

        public async Task PostChatMessage(string roomId, string message)
        {
            var request = CreateAuthorizedRequest(HttpMethod.Post, new Uri($"{ServerAddress}/api/v1/chat.postMessage"));
            var content = new
            {
                roomId = roomId,
                text = message
            };
            var stringContent = new StringContent(JsonConvert.SerializeObject(content));
            stringContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            request.Content = stringContent;
            var res = await httpClient.SendAsync(request);
        }

        public async Task<BitmapImage> GetImage(string imageUrl)
        {
            var request = CreateAuthorizedRequest(HttpMethod.Get, new Uri($"{ServerAddress}{imageUrl}"));
            var res = await httpClient.SendAsync(request);
            var content = await res.Content.ReadAsStreamAsync();
            var img = new BitmapImage();
            var memStream = new MemoryStream();
            await content.CopyToAsync(memStream);
            memStream.Position = 0;
            img.SetSource(memStream.AsRandomAccessStream());
            return img;
        }

        public async Task<MemoryStream> GetFile(string fileUrl)
        {
            var request = CreateAuthorizedRequest(HttpMethod.Get, new Uri($"{ServerAddress}{fileUrl}"));
            var res = await httpClient.SendAsync(request);
            var content = await res.Content.ReadAsStreamAsync();
            var memStream = new MemoryStream();
            await content.CopyToAsync(memStream);
            return memStream;
        }

        public async Task<IEnumerable<Message>> GetPrivateGroupHistory(string roomId, int offset = 0, int count = 20)
        {
            var request = CreateAuthorizedRequest(HttpMethod.Get, new Uri($"{ServerAddress}/api/v1/groups.history?roomId={roomId}&offset={offset}&count={count}"));
            var response = await httpClient.SendAsync(request);
            var responseContent = JsonConvert.DeserializeObject<ChatHistoryResponse>(await response.Content.ReadAsStringAsync());
            var messages = new List<Message>();
            foreach (var message in responseContent.messages)
            {
                messages.Insert(0, new Message(message));
            }
            return messages;
        }
        #endregion

        #region event handlers
        private void ServerAddressChangedHandler(ServerAddress payload)
        {
            ServerAddress = payload.HttpAddress;
        }
        #endregion
    }
}
