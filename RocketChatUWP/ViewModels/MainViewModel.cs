﻿using System;
using System.ComponentModel;
using System.Threading.Tasks;
using Prism.Commands;
using Prism.Events;
using Prism.Windows.Mvvm;
using Prism.Windows.Navigation;
using RocketChatUWP.Core.Api;
using RocketChatUWP.Core.Constants;
using RocketChatUWP.Core.Events.Login;
using RocketChatUWP.Core.Helpers;
using RocketChatUWP.Core.Services;
using RocketChatUWP.Views;
using Windows.UI.Xaml.Controls;

namespace RocketChatUWP.ViewModels
{
    public class MainViewModel : ViewModelBase, INotifyPropertyChanged
    {
        #region private members
        private readonly IEventAggregator eventAggregator;
        private readonly IRocketChatRestApi rocketChat;
        private readonly INavigationService navigationService;
        private readonly IRocketChatRealtimeApi rocketChatRealtime;
        private readonly IToastNotificationsService toastService;
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
        public DelegateCommand SettingsCommand { get; set; }
        #endregion

        #region ctor
        public MainViewModel(
            IEventAggregator eventAggregator,
            IRocketChatRestApi rocketChat,
            INavigationService navigationService,
            IRocketChatRealtimeApi rocketChatRealtime,
            IToastNotificationsService toastService
            )
        {
            this.eventAggregator = eventAggregator;
            this.rocketChat = rocketChat;
            this.navigationService = navigationService;
            this.rocketChatRealtime = rocketChatRealtime;
            this.toastService = toastService;
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
            SettingsCommand = new DelegateCommand(OnSettings);
        }

        private async void OnLogin()
        {
            bool validateServerAddresses = true;
            if (rocketChat.ServerAddress == string.Empty)
            {
                toastService.ShowErrorToastNotification("Błąd połączenia", "Nie ustawiono adresu serwera http Rocket.Chat.");
                validateServerAddresses = false;
            }
            if(rocketChatRealtime.ServerAddress == string.Empty)
            {
                toastService.ShowErrorToastNotification("Błąd połączenia", "Nie ustawiono adresu serwera websocket Rocket.Chat.");
                validateServerAddresses = false;
            }
            if (!validateServerAddresses)
                return;

            ButtonsEnabled = false;
            bool result;
            try
            {
                result = await rocketChat.Login(Username, Password);
                await rocketChatRealtime.Connect();
                ButtonsEnabled = true;
            }
            catch
            {
                ButtonsEnabled = true;
                return;
            }
            if (result && rocketChatRealtime.IsConnected)
                navigationService.Navigate(PageTokens.ChatPage, "");
            else
            {
                if(!rocketChatRealtime.IsConnected)
                {
                    rocketChat.Logout();
                    return;
                }

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

        private async void OnSettings()
        {
            var dialog = new SettingsDialog();
            var result = await dialog.ShowAsync();
            if(result == ContentDialogResult.Secondary)
            {
                eventAggregator.GetEvent<ServerAddressChangedEvent>().Publish(dialog.Address);
                await ServerAddressHelper.SetServerAddressAndSave(dialog.Address);
            }                
        }
        #endregion
    }
}
