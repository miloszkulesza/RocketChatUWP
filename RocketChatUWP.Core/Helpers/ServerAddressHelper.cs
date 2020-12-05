using Newtonsoft.Json;
using RocketChatUWP.Core.Models;
using System;
using System.Threading.Tasks;
using Windows.Storage;

namespace RocketChatUWP.Core.Helpers
{
    public static class ServerAddressHelper
    {
        public async static Task<ServerAddress> GetServerAddress()
        {
            StorageFolder storageFolder = ApplicationData.Current.LocalFolder;
            StorageFile file = await storageFolder.CreateFileAsync("settings.json", CreationCollisionOption.OpenIfExists);
            var address = new ServerAddress();

            if (file.IsAvailable)
            {
                dynamic content = JsonConvert.DeserializeObject(await FileIO.ReadTextAsync(file));
                if (content != null)
                {
                    if (content.settings != null)
                    {
                        if (content.settings.restServerAddress != null)
                            address.HttpAddress = content.settings.restServerAddress;

                        if (content.settings.websocketAddress != null)
                            address.WebsocketAddress = content.settings.websocketAddress;
                    }
                }
            }

            return address;
        }

        public async static Task SetServerAddressAndSave(ServerAddress address)
        {
            StorageFolder storageFolder = ApplicationData.Current.LocalFolder;
            StorageFile file = await storageFolder.CreateFileAsync("settings.json", CreationCollisionOption.OpenIfExists);
            var content = new
            {
                settings = new
                {
                    restServerAddress = address.HttpAddress,
                    websocketAddress = address.WebsocketAddress
                }
            };
            var json = JsonConvert.SerializeObject(content);
            await FileIO.WriteTextAsync(file, json);
        }
    }
}
