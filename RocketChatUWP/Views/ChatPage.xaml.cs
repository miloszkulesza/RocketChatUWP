using RocketChatUWP.ViewModels;
using Windows.UI.Xaml.Controls;

//Szablon elementu Pusta strona jest udokumentowany na stronie https://go.microsoft.com/fwlink/?LinkId=234238

namespace RocketChatUWP.Views
{
    /// <summary>
    /// Pusta strona, która może być używana samodzielnie lub do której można nawigować wewnątrz ramki.
    /// </summary>
    public sealed partial class ChatPage : Page
    {
        public ChatPage()
        {
            this.InitializeComponent();
        }

        private void KeyboardAccelerator_Invoked(Windows.UI.Xaml.Input.KeyboardAccelerator sender, Windows.UI.Xaml.Input.KeyboardAcceleratorInvokedEventArgs args)
        {
            if((DataContext as ChatViewModel).SendMessageCommand.CanExecute())
                (DataContext as ChatViewModel).SendMessageCommand.Execute();
        }
    }
}
