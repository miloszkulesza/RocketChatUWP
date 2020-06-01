using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection.Metadata;
using System.Threading.Tasks;
using System.Xml.Serialization;
using Microsoft.Graphics.Canvas.Text;
using Microsoft.Practices.Unity;
using Prism.Commands;
using Prism.Events;
using Prism.Windows.Mvvm;
using Prism.Windows.Navigation;
using RocketChatUWP.Core.Api;
using RocketChatUWP.Core.Constants;
using RocketChatUWP.Core.Events.Login;

namespace RocketChatUWP.ViewModels
{
    public class MainViewModel : ViewModelBase, INotifyPropertyChanged
    {
        #region private members
        private readonly IEventAggregator eventAggregator;
        private readonly IRocketChatRestApi rocketChat;
        private readonly INavigationService navigationService;
        private readonly IRocketChatRealtimeApi rocketChatRealtime;
        private readonly IUnityContainer unityContainer;
        #endregion

        #region properties
        private string username;
        public string Username
        {
            get { return username; }
            set { SetProperty(ref username, value); }
        }

        private string password;
        public string Password
        {
            get { return password; }
            set { SetProperty(ref password, value); }
        }

        private bool buttonsEnabled;
        public bool ButtonsEnabled
        {
            get { return buttonsEnabled; }
            set { SetProperty(ref buttonsEnabled, value); }
        }

        private bool isPopupOpened;
        public bool IsPopupOpened
        {
            get { return isPopupOpened; }
            set { SetProperty(ref isPopupOpened, value); }
        }
        #endregion

        #region commands
        public DelegateCommand LoginCommand { get; set; }
        public DelegateCommand ClosePopupCommand { get; set; }
        #endregion

        #region ctor
        public MainViewModel(
            IEventAggregator eventAggregator,
            IRocketChatRestApi rocketChat,
            INavigationService navigationService,
            IRocketChatRealtimeApi rocketChatRealtime,
            IUnityContainer unityContainer
            )
        {
            this.unityContainer = unityContainer;
            this.eventAggregator = eventAggregator;
            this.rocketChat = rocketChat;
            this.navigationService = navigationService;
            this.rocketChatRealtime = rocketChatRealtime;
            RegisterCommands();
            ButtonsEnabled = true;
            IsPopupOpened = false;
        }
        #endregion

        #region private methods
        private void RegisterCommands()
        {
            LoginCommand = new DelegateCommand(OnLogin);
            ClosePopupCommand = new DelegateCommand(OnClosePopup);
        }

        private async void OnLogin()
        {
            ButtonsEnabled = false;
            bool result;
            try
            {
                result = await rocketChat.Login(Username, Password);
                ButtonsEnabled = true;
            }
            catch
            {
                ButtonsEnabled = true;
                return;
            }
            if (result)
            {
                rocketChatRealtime.Connect();
                navigationService.Navigate(PageTokens.ChatPage, "");
            }
            else
            {
                Username = string.Empty;
                Password = string.Empty;
                IsPopupOpened = true;
                await Task.Delay(TimeSpan.FromSeconds(3));
                IsPopupOpened = false;
            }
        }


        private void OnClosePopup()
        {
            IsPopupOpened = false;
        }
        #endregion
    }
}
