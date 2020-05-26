using Prism.Commands;
using Prism.Events;
using Prism.Windows.Mvvm;
using RocketChatUWP.Core.Api;
using RocketChatUWP.Core.ApiModels;
using RocketChatUWP.Core.Events.Websocket;
using RocketChatUWP.Core.Models;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using Windows.UI;
using Windows.UI.Core;
using Windows.UI.Xaml.Media;

namespace RocketChatUWP.ViewModels
{
    public class ChatViewModel : ViewModelBase
    {
        #region private members
        private readonly IRocketChatRestApi rocketChatRest;
        private readonly IEventAggregator eventAggregator;
        private readonly IRocketChatRealtimeApi realtimeApi;
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
        #endregion

        #region commands
        public DelegateCommand OnlineStatusCommand { get; set; }
        public DelegateCommand OfflineStatusCommand { get; set; }
        public DelegateCommand AwayStatusCommand { get; set; }
        public DelegateCommand BusyStatusCommand { get; set; }
        #endregion

        #region ctor
        public ChatViewModel(
            IRocketChatRestApi rocketChatRest,
            IEventAggregator eventAggregator,
            IRocketChatRealtimeApi realtimeApi)
        {
            this.rocketChatRest = rocketChatRest;
            this.eventAggregator = eventAggregator;
            this.realtimeApi = realtimeApi;
            GetRooms();
            LoggedUser = this.rocketChatRest.User;
            ChangeCurrentStatus(LoggedUser.Status);
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
        }

        private void RegisterSubscriptions()
        {
            eventAggregator.GetEvent<UserConnectionStatusChangedEvent>().Subscribe(UserConnectionStatusChangedHandler);
        }

        private void ChangeCurrentStatus(string status)
        {
            switch(status)
            {
                case "offline":
                    CurrentStatusMenuText = "Niewidoczny";
                    CurrentStatusMenuDotColor = new SolidColorBrush(Colors.Gray);
                    break;
                case "online":
                    CurrentStatusMenuText = "Online";
                    CurrentStatusMenuDotColor = new SolidColorBrush(Colors.LightGreen);
                    break;
                case "away":
                    CurrentStatusMenuText = "Zaraz wracam";
                    CurrentStatusMenuDotColor = new SolidColorBrush(Colors.Orange);
                    break;
                case "busy":
                    CurrentStatusMenuText = "Zajęty";
                    CurrentStatusMenuDotColor = new SolidColorBrush(Colors.Red);
                    break;
            }
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
                    ChangeCurrentStatus(LoggedUser.Status);
                });
                
            }
        }
        #endregion
    }
}
