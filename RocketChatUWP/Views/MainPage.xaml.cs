using RocketChatUWP.ViewModels;
using Windows.UI.Xaml.Controls;

namespace RocketChatUWP.Views
{
    public sealed partial class MainPage : Page
    {
        private MainViewModel ViewModel => DataContext as MainViewModel;

        public MainPage()
        {
            InitializeComponent();
        }

        private void KeyboardAccelerator_Invoked(Windows.UI.Xaml.Input.KeyboardAccelerator sender, Windows.UI.Xaml.Input.KeyboardAcceleratorInvokedEventArgs args)
        {
            ViewModel.LoginCommand.Execute();
        }
    }
}
