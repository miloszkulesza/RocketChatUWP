using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Prism.Events;
using RocketChatUWP.Core.Services;
using System;
using System.Diagnostics;
using Windows.Networking.Sockets;
using Windows.Storage;
using Windows.Storage.Streams;

namespace RocketChatUWP.Core.Api
{
    public class RocketChatRealtimeApi : IRocketChatRealtimeApi
    {
        private string serverAddress;
        private string guid = Guid.NewGuid().ToString();
        private readonly MessageWebSocket socket;
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

        public async void Connect()
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
            using (var dataWriter = new DataWriter(socket.OutputStream))
            {
                dataWriter.WriteString(jsonSerialized);
                await dataWriter.StoreAsync();
                dataWriter.DetachStream();
            }
            Login();
        }

        private async void Login()
        {
            var loginJson = new JObject(new JProperty("msg", "method"),
                                        new JProperty("method", "login"),
                                        new JProperty("id", guid),
                                        new JProperty("params", new JArray(new JObject(new JProperty("resume", rocketChatRest.User.AuthToken)))));
            var jsonSerialized = JsonConvert.SerializeObject(loginJson);
            using (var dataWriter = new DataWriter(socket.OutputStream))
            {
                dataWriter.WriteString(jsonSerialized);
                await dataWriter.StoreAsync();
                dataWriter.DetachStream();
            }
        }


        private void OnMessageReceived(MessageWebSocket sender, MessageWebSocketMessageReceivedEventArgs args)
        {
            using (DataReader dataReader = args.GetDataReader())
            {
                dataReader.UnicodeEncoding = UnicodeEncoding.Utf8;
                string messageReceived = dataReader.ReadString(dataReader.UnconsumedBufferLength);
                Debug.WriteLine($"[{DateTime.Now}]Message received from MessageWebSocket: {messageReceived}");
                dynamic message = JsonConvert.DeserializeObject(messageReceived);
                if(message.msg != null && message.msg == "ping")
                {
                    PongRequest();
                }
            }
        }

        private async void PongRequest()
        {
            var pongJson = new
            {
                msg = "pong"
            };
            var serializedJson = JsonConvert.SerializeObject(pongJson);
            using (var dataWriter = new DataWriter(socket.OutputStream))
            {
                dataWriter.WriteString(serializedJson);
                await dataWriter.StoreAsync();
                dataWriter.DetachStream();
            }
            Debug.WriteLine($"[{DateTime.Now}]Message sent to MessageWebSocket: {serializedJson}");
        }
    }
}
