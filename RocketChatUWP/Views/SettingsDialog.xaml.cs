using RocketChatUWP.Core.Helpers;
using RocketChatUWP.Core.Models;
using Windows.UI.Xaml.Controls;

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
