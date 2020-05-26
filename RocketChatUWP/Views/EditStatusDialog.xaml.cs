using Windows.UI;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Shapes;

//Szablon elementu Pusta strona jest udokumentowany na stronie https://go.microsoft.com/fwlink/?LinkId=234238

namespace RocketChatUWP.Views
{
    /// <summary>
    /// Pusta strona, która może być używana samodzielnie lub do której można nawigować wewnątrz ramki.
    /// </summary>
    public sealed partial class EditStatusDialog : ContentDialog
    {
        public string Message { get; set; }

        public EditStatusDialog(string message)
        {
            this.InitializeComponent();
            if (message == "Online" || message == "Niewidoczny" || message == "Zaraz wracam" || message == "Zajęty")
            {
                Message = null;
                this.message.Text = string.Empty;
            }
            else
            {
                Message = message;
                this.message.Text = Message;
            }           
        }

        private void message_TextChanged(object sender, TextChangedEventArgs e)
        {
            Message = message.Text;
        }
    }
}
