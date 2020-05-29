using Prism.Commands;
using Prism.Events;
using Prism.Windows.Mvvm;
using Prism.Windows.Navigation;
using RocketChatUWP.Core.Api;
using RocketChatUWP.Core.ApiModels;
using RocketChatUWP.Core.Constants;
using RocketChatUWP.Core.Events.Websocket;
using RocketChatUWP.Core.Models;
using RocketChatUWP.Views;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using Windows.UI;
using Windows.UI.Core;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;

namespace RocketChatUWP.ViewModels
{
    public class ChatViewModel : ViewModelBase
    {
        #region private members
        private readonly IRocketChatRestApi rocketChatRest;
        private readonly IEventAggregator eventAggregator;
        private readonly IRocketChatRealtimeApi realtimeApi;
        private readonly INavigationService navigationService;
        #endregion

        #region properties
        private ObservableCollection<Room> channels;
        public ObservableCollection<Room> Channels
        {
            get { return channels; }
            set { SetProperty(ref channels, value); }
        }

        private ObservableCollection<Room> discussions;
        public ObservableCollection<Room> Discussions
        {
            get { return discussions; }
            set { SetProperty(ref discussions, value); }
        }

        private ObservableCollection<Room> privateMessages;
        public ObservableCollection<Room> PrivateMessages
        {
            get { return privateMessages; }
            set { SetProperty(ref privateMessages, value); }
        }

        private ObservableCollection<Room> privateDiscussions;
        public ObservableCollection<Room> PrivateDiscussions
        {
            get { return privateDiscussions; }
            set { SetProperty(ref privateDiscussions, value); }
        }

        private ObservableCollection<Room> privateGroups;
        public ObservableCollection<Room> PrivateGroups
        {
            get { return privateGroups; }
            set { SetProperty(ref privateGroups, value); }
        }

        private Room selectedChannel;
        public Room SelectedChannel
        {
            get { return selectedChannel; }
            set
            {
                SetProperty(ref selectedChannel, null);
                SetProperty(ref selectedChannel, value);
            }
        }

        private User loggedUser;
        public User LoggedUser
        {
            get { return loggedUser; }
            set { SetProperty(ref loggedUser, value); }
        }

        private string currentStatusMenuText;
        public string CurrentStatusMenuText
        {
            get { return currentStatusMenuText; }
            set { SetProperty(ref currentStatusMenuText, value); }
        }

        private Brush currentStatusMenuDotColor;
        public Brush CurrentStatusMenuDotColor
        {
            get { return currentStatusMenuDotColor; }
            set { SetProperty(ref currentStatusMenuDotColor, value); }
        }

        private ObservableCollection<Message> messages;
        public ObservableCollection<Message> Messages
        {
            get { return messages; }
            set { SetProperty(ref messages, value); }
        }

        private ObservableCollection<User> users;
        public ObservableCollection<User> Users
        {
            get { return users; }
            set { SetProperty(ref users, value); }
        }
        #endregion

        #region commands
        public DelegateCommand OnlineStatusCommand { get; set; }
        public DelegateCommand OfflineStatusCommand { get; set; }
        public DelegateCommand AwayStatusCommand { get; set; }
        public DelegateCommand BusyStatusCommand { get; set; }
        public DelegateCommand EditStatusCommand { get; set; }
        public DelegateCommand SelectedChannelCommand { get; set; }
        #endregion

        #region ctor
        public ChatViewModel(
            IRocketChatRestApi rocketChatRest,
            IEventAggregator eventAggregator,
            IRocketChatRealtimeApi realtimeApi,
            INavigationService navigationService)
        {
            this.rocketChatRest = rocketChatRest;
            this.eventAggregator = eventAggregator;
            this.realtimeApi = realtimeApi;
            this.navigationService = navigationService;
            GetRooms();
            LoggedUser = this.rocketChatRest.User;
            ChangeCurrentStatus(new UserConnectionStatusNotification { fields = new UserConnectionStatusFields { status = LoggedUser.Status , message = LoggedUser.StatusText } });
            GetUsers();
            RegisterCommands();
            RegisterSubscriptions();
        }
        #endregion

        #region private methods
        private void RegisterCommands()
        {
            OnlineStatusCommand = new DelegateCommand(OnOnlineStatus);
            OfflineStatusCommand = new DelegateCommand(OnOfflineStatus);
            AwayStatusCommand = new DelegateCommand(OnAwayStatus);
            BusyStatusCommand = new DelegateCommand(OnBusyStatus);
            EditStatusCommand = new DelegateCommand(OnEditStatus);
            SelectedChannelCommand = new DelegateCommand(OnSelectedChannel);
        }

        private async void OnSelectedChannel()
        {
            if(SelectedChannel != null)
            {
                var messages = await rocketChatRest.GetChannelHistory(SelectedChannel.Id);
                foreach(var message in messages)
                {
                    message.User = Users.FirstOrDefault(x => x.Id == message.User.Id);
                }
                Messages = new ObservableCollection<Message>(messages);
            }
        }

        #region commands implementation
        private void OnBusyStatus()
        {
            realtimeApi.SetUserStatus("busy");
        }

        private void OnAwayStatus()
        {
            realtimeApi.SetUserStatus("away");
        }

        private void OnOfflineStatus()
        {
            realtimeApi.SetUserStatus("offline");
        }

        private void OnOnlineStatus()
        {
            realtimeApi.SetUserStatus("online");
        }

        private async void OnEditStatus()
        {
            var dialog = new EditStatusDialog(CurrentStatusMenuText);
            var result = await dialog.ShowAsync();
            string message = string.Empty;
            bool isCanceled = false;
            switch(result)
            {
                case ContentDialogResult.Primary:
                    isCanceled = true;
                    break;
                case ContentDialogResult.Secondary:
                    message = dialog.Message;
                    break;
            }
            if (!isCanceled)
            {
                rocketChatRest.SetUserStatus(message);
            }
        }
        #endregion

        private async void GetRooms()
        {
            var rooms = await rocketChatRest.GetRooms();
            Channels = new ObservableCollection<Room>(rooms.Where(x => x.T == "c" && x.Topic == null).OrderByDescending(x => x.UpdatedAt).ToList());
            PrivateMessages = new ObservableCollection<Room>(rooms.Where(x => x.T == "d").OrderByDescending(x => x.UpdatedAt).ToList());
            foreach(var priv in PrivateMessages)
            {
                foreach(var username in priv.Usernames)
                {
                    if (rocketChatRest.User.Username != username)
                        priv.DiscussionName = username;
                }
            }
            Discussions = new ObservableCollection<Room>(rooms.Where(x => x.Topic != null).OrderByDescending(x => x.UpdatedAt).ToList());
            foreach(var discussion in Discussions)
            {
                discussion.Name = discussion.DiscussionName;
            }
        }

        private void RegisterSubscriptions()
        {
            eventAggregator.GetEvent<UserConnectionStatusChangedEvent>().Subscribe(UserConnectionStatusChangedHandler);
        }

        private void ChangeCurrentStatus(UserConnectionStatusNotification obj)
        {
            switch(obj.fields.status)
            {
                case "offline":
                    CurrentStatusMenuDotColor = new SolidColorBrush(Colors.Gray);
                    break;
                case "online":
                    CurrentStatusMenuDotColor = new SolidColorBrush(Colors.LightGreen);
                    break;
                case "away":
                    CurrentStatusMenuDotColor = new SolidColorBrush(Colors.Orange);
                    break;
                case "busy":
                    CurrentStatusMenuDotColor = new SolidColorBrush(Colors.Red);
                    break;
                default:
                    break;
            }
            if(obj.fields.message != null)
            {
                if(obj.fields.message == string.Empty)
                {
                    switch(obj.fields.status)
                    {
                        case "offline":
                            CurrentStatusMenuText = "Niewidoczny";
                            break;
                        case "online":
                            CurrentStatusMenuText = "Online";
                            break;
                        case "away":
                            CurrentStatusMenuText = "Zaraz wracam";
                            break;
                        case "busy":
                            CurrentStatusMenuText = "Zajęty";
                            break;
                    }
                }
                else
                    CurrentStatusMenuText = obj.fields.message;
            }
        }

        private async void GetUsers()
        {
            Users = new ObservableCollection<User>(await rocketChatRest.GetUsersList());
        }
        #endregion

        #region event handlers
        private async void UserConnectionStatusChangedHandler(UserConnectionStatusNotification obj)
        {
            if(obj.fields.userId == LoggedUser.Id)
            {
                await Windows.ApplicationModel.Core.CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                {
                    LoggedUser.Status = obj.fields.status;
                    ChangeCurrentStatus(obj);
                });
                
            }
        }
        #endregion
    }
}
