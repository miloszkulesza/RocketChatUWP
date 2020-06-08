﻿using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Prism.Events;
using RocketChatUWP.Core.ApiModels;
using RocketChatUWP.Core.Events.Websocket;
using RocketChatUWP.Core.Services;
using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Windows.Networking.Sockets;
using Windows.Storage;
using Windows.Storage.Streams;

namespace RocketChatUWP.Core.Api
{
    public class RocketChatRealtimeApi : IRocketChatRealtimeApi
    {
        private string serverAddress;
        private string guid = Guid.NewGuid().ToString();
        private MessageWebSocket socket;
        private readonly IEventAggregator eventAggregator;
        private readonly IRocketChatRestApi rocketChatRest;
        private readonly IToastNotificationsService toastService;

        public RocketChatRealtimeApi(
            IEventAggregator eventAggregator,
            IRocketChatRestApi rocketChatRest,
            IToastNotificationsService toastService)
        {
            this.eventAggregator = eventAggregator;
            this.rocketChatRest = rocketChatRest;
            this.toastService = toastService;
            SetServerAddress();
            socket = new MessageWebSocket();
            socket.MessageReceived += OnMessageReceived;
        }

        private async void SetServerAddress()
        {
            StorageFolder storageFolder = ApplicationData.Current.LocalFolder;
            StorageFile file = await storageFolder.CreateFileAsync("settings.json", CreationCollisionOption.OpenIfExists);
            if (file.IsAvailable)
            {
                dynamic content = JsonConvert.DeserializeObject(await FileIO.ReadTextAsync(file));
                if (content.settings.websocketServerAddress != null)
                    serverAddress = content.settings.websocketServerAddress;
                else
                {
                    content.settings.websocketServerAddress = "ws://localhost:3000/websocket";
                    var json = JsonConvert.SerializeObject(content);
                    FileIO.WriteTextAsync(file, json);
                    serverAddress = content.settings.websocketServerAddress;
                }
            }
        }

        public async Task Connect()
        {
            try
            {
                await socket.ConnectAsync(new Uri(serverAddress));
            }
            catch
            {
                toastService.ShowErrorToastNotification("Błąd połączenia", "Nie udało się nawiązać połączenia z websocketem Rocket.Chat.");
                return;
            }
            var connectJson = new JObject(new JProperty("msg", "connect"),
                                          new JProperty("version", "1"),
                                          new JProperty("support", new JArray("1")));

            var jsonSerialized = JsonConvert.SerializeObject(connectJson);
            await SendMessage(jsonSerialized);
            await Login();
            await SendSubscriptionsRequests();
        }

        private async Task Login()
        {
            var loginJson = new JObject(new JProperty("msg", "method"),
                                        new JProperty("method", "login"),
                                        new JProperty("id", guid),
                                        new JProperty("params", new JArray(new JObject(new JProperty("resume", rocketChatRest.User.AuthToken)))));
            var jsonSerialized = JsonConvert.SerializeObject(loginJson);
            await SendMessage(jsonSerialized);
        }


        private async void OnMessageReceived(MessageWebSocket sender, MessageWebSocketMessageReceivedEventArgs args)
        {
            using (DataReader dataReader = args.GetDataReader())
            {
                dataReader.UnicodeEncoding = UnicodeEncoding.Utf8;
                string messageReceived = dataReader.ReadString(dataReader.UnconsumedBufferLength);
                Debug.WriteLine($"[{DateTime.Now}]Message received from MessageWebSocket: {messageReceived}");
                dynamic message = JsonConvert.DeserializeObject(messageReceived);
                if(message.msg != null && message.msg == "ping")
                {
                    await PongRequest();
                }
                if(message.msg != null)
                {
                    if(message.msg == "changed" && message.collection == "stream-notify-logged")
                        HandleUserConnectionStatusChange(messageReceived);
                    if(message.msg == "changed" && message.collection == "stream-room-messages")
                    {
                        if(message.fields.eventName == "__my_messages__")
                            HandleUserNotifyStream(messageReceived);
                    }
                }
            }
        }

        private async Task PongRequest()
        {
            var pongJson = new
            {
                msg = "pong"
            };
            var serializedJson = JsonConvert.SerializeObject(pongJson);
            await SendMessage(serializedJson);
        }

        private async Task SendSubscriptionsRequests()
        {
            var subscriptionJson = new JObject(new JProperty("msg", "sub"),
                                               new JProperty("id", Guid.NewGuid().ToString()),
                                               new JProperty("name", "stream-notify-logged"),
                                               new JProperty("params", new JArray("user-status", false)));
            var subscriptonSerialized = JsonConvert.SerializeObject(subscriptionJson);
            await SendMessage(subscriptonSerialized);


                subscriptionJson = new JObject(new JProperty("msg", "sub"),
                                           new JProperty("id", Guid.NewGuid().ToString()),
                                           new JProperty("name", "stream-room-messages"),
                                           new JProperty("params", new JArray("__my_messages__", false)));
                subscriptonSerialized = JsonConvert.SerializeObject(subscriptionJson);
                await SendMessage(subscriptonSerialized);
            
            subscriptionJson = new JObject(new JProperty("msg", "sub"),
                                           new JProperty("id", Guid.NewGuid().ToString()),
                                           new JProperty("name", "stream-notify-user"),
                                           new JProperty("params", new JArray($"{rocketChatRest.User.Id}/notification", false)));
            subscriptonSerialized = JsonConvert.SerializeObject(subscriptionJson);
            await SendMessage(subscriptonSerialized);
        }

        private void HandleUserConnectionStatusChange(string message)
        {
            var notification = JsonConvert.DeserializeObject<UserConnectionStatusNotification>(message);
            dynamic array = notification.fields.args[0];
            notification.fields.userId = array[0].ToString();
            notification.fields.username = array[1].ToString();
            switch (int.Parse(array[2].ToString()))
            {
                case 0:
                    notification.fields.status = "offline";
                    break;
                case 1:
                    notification.fields.status = "online";
                    break;
                case 2:
                    notification.fields.status = "away";
                    break;
                case 3:
                    notification.fields.status = "busy";
                    break;
                default:
                    break;
            }
            if(array[3] != null || array[3].ToString() == string.Empty)
                notification.fields.message = array[3].ToString();
            eventAggregator.GetEvent<UserConnectionStatusChangedEvent>().Publish(notification);
        }

        public async Task SendMessage(string message)
        {
            using (var dataWriter = new DataWriter(socket.OutputStream))
            {
                dataWriter.WriteString(message);
                await dataWriter.StoreAsync();
                dataWriter.DetachStream();
                Debug.WriteLine($"[{DateTime.Now}]Message sent to MessageWebSocket: {message}");
            }
        }

        public async void SetUserStatus(string status)
        {
            var json = new JObject(new JProperty("msg", "method"),
                                   new JProperty("method", $"UserPresence:setDefaultStatus"),
                                   new JProperty("id", guid),
                                   new JProperty("params", new JArray(status)));
            var serializedJson = JsonConvert.SerializeObject(json);
            await SendMessage(serializedJson);
        }

        public void DisposeSocket()
        {
            socket.Dispose();
            socket = new MessageWebSocket();
            socket.MessageReceived += OnMessageReceived;
            guid = Guid.NewGuid().ToString();
        }

        private void HandleUserNotifyStream(string messageReceived)
        {
            var notification = JsonConvert.DeserializeObject<NewMessageNotification>(messageReceived);
            eventAggregator.GetEvent<NewMessageEvent>().Publish(notification);
        }
    }
}
