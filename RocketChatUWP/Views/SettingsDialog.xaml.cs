using Newtonsoft.Json;
using Prism.Events;
using RocketChatUWP.Core.Events.Login;
using RocketChatUWP.Core.Helpers;
using RocketChatUWP.Core.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// Szablon elementu Kontrolka użytkownika jest udokumentowany na stronie https://go.microsoft.com/fwlink/?LinkId=234236

namespace RocketChatUWP.Views
{
    public sealed partial class SettingsDialog : ContentDialog
    {
        public ServerAddress Address { get; set; }

        public SettingsDialog()
        {
            this.InitializeComponent();
            Address = new ServerAddress();
            GetServerAddress();
        }

        private async void GetServerAddress()
        {
            var address = await ServerAddressHelper.GetServerAddress();
            Address.HttpAddress = address.HttpAddress;
            Address.WebsocketAddress = address.WebsocketAddress;
            http.Text = Address.HttpAddress;
            websocket.Text = Address.WebsocketAddress;
        }

        private void http_TextChanged(object sender, TextChangedEventArgs e)
        {
            Address.HttpAddress = http.Text;
        }

        private void websocket_TextChanged(object sender, TextChangedEventArgs e)
        {
            Address.WebsocketAddress = websocket.Text;
        }
    }
}
