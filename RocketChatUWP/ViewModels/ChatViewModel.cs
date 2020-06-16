using Prism.Commands;
using Prism.Events;
using Prism.Windows.Mvvm;
using Prism.Windows.Navigation;
using RocketChatUWP.Core.Api;
using RocketChatUWP.Core.ApiModels;
using RocketChatUWP.Core.Constants;
using RocketChatUWP.Core.Events.Websocket;
using RocketChatUWP.Core.Helpers;
using RocketChatUWP.Core.Models;
using RocketChatUWP.Core.Services;
using RocketChatUWP.Views;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Windows.UI;
using Windows.UI.Core;
using Windows.UI.Text;
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
        private readonly IToastNotificationsService notificationService;
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

        private ObservableCollection<Room> directConversations;
        public ObservableCollection<Room> DirectConversations
        {
            get { return directConversations; }
            set { SetProperty(ref directConversations, value); }
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

        private string messageText;
        public string MessageText
        {
            get { return messageText; }
            set { SetProperty(ref messageText, value); }
        }
        #endregion

        #region commands
        public DelegateCommand OnlineStatusCommand { get; set; }
        public DelegateCommand OfflineStatusCommand { get; set; }
        public DelegateCommand AwayStatusCommand { get; set; }
        public DelegateCommand BusyStatusCommand { get; set; }
        public DelegateCommand EditStatusCommand { get; set; }
        public DelegateCommand SelectedChannelCommand { get; set; }
        public DelegateCommand LogoutCommand { get; set; }
        public DelegateCommand SelectedDirectConversationCommand { get; set; }
        public DelegateCommand SendMessageCommand { get; set; }
        #endregion

        #region ctor
        public ChatViewModel(
            IRocketChatRestApi rocketChatRest,
            IEventAggregator eventAggregator,
            IRocketChatRealtimeApi realtimeApi,
            INavigationService navigationService,
            IToastNotificationsService notificationService)
        {
            this.rocketChatRest = rocketChatRest;
            this.eventAggregator = eventAggregator;
            this.realtimeApi = realtimeApi;
            this.navigationService = navigationService;
            this.notificationService = notificationService;
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
            LogoutCommand = new DelegateCommand(OnLogout);
            SelectedDirectConversationCommand = new DelegateCommand(OnSelectedDirectConversation);
            SendMessageCommand = new DelegateCommand(OnSendMessage, CanSendMessage);
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
            var dialog = new EditStatusDialog(LoggedUser.UserPresence.StatusText);
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

        private async void OnSelectedChannel()
        {
            if (SelectedChannel != null)
            {
                SelectedChannel.HasUnreadedMessages = false;
                SelectedChannel.ChannelFontWeight = FontWeights.Normal;
                var messages = await rocketChatRest.GetChannelHistory(SelectedChannel.Id);
                foreach (var message in messages)
                {
                    message.User = Users.FirstOrDefault(x => x.Id == message.User.Id);
                    if(message.Attachments != null && message.Attachments.Length > 0)
                    {
                        foreach(var attachment in message.Attachments)
                        {
                            attachment.ImagePreview = await rocketChatRest.GetImage(attachment.ImageUrl);
                        }
                    }
                }
                Messages = new ObservableCollection<Message>(messages);
            }
        }

        private void OnLogout()
        {
            rocketChatRest.Logout();
            realtimeApi.DisposeSocket();
            navigationService.Navigate(PageTokens.MainPage, new { logout = true });
        }

        private async void OnSelectedDirectConversation()
        {
            if (SelectedChannel != null)
            {
                SelectedChannel.HasUnreadedMessages = false;
                SelectedChannel.ChannelFontWeight = FontWeights.Normal;
                var messages = await rocketChatRest.GetDirectMessages(SelectedChannel.Id);
                foreach (var message in messages)
                {
                    message.User = Users.FirstOrDefault(x => x.Id == message.User.Id);
                }
                Messages = new ObservableCollection<Message>(messages);
            }
        }

        private void OnSendMessage()
        {
            rocketChatRest.PostChatMessage(SelectedChannel.Id, MessageText);
            MessageText = string.Empty;
        }
        #endregion

        #region can execute command
        private bool CanSendMessage()
        {
            return !string.IsNullOrEmpty(MessageText) && SelectedChannel != null;
        }
        #endregion

        #region other methods
        private async Task GetRooms()
        {
            var rooms = await rocketChatRest.GetRooms();
            Channels = new ObservableCollection<Room>(rooms.Where(x => x is Channel).ToList());
            DirectConversations = new ObservableCollection<Room>(rooms.Where(x => x is DirectConversation).ToList());
            foreach(var conversation in DirectConversations)
            {
                var conversationCasted = conversation as DirectConversation;
                var userId = conversationCasted.UserIds.FirstOrDefault(x => x != LoggedUser.Id);
                conversationCasted.User = Users.FirstOrDefault(x => x.Id == userId);
                conversationCasted.Name = conversationCasted.User.Username;
                conversationCasted.Avatar = conversationCasted.User.Avatar;
            }
            Discussions = new ObservableCollection<Room>(rooms.Where(x => x is Discussion).ToList());
            PrivateGroups = new ObservableCollection<Room>();
        }

        private void RegisterSubscriptions()
        {
            eventAggregator.GetEvent<UserConnectionStatusChangedEvent>().Subscribe(UserConnectionStatusChangedHandler);
            eventAggregator.GetEvent<NewMessageEvent>().Subscribe(NewMessageHandler);
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
                            LoggedUser.UserPresence.StatusText = "Niewidoczny";
                            break;
                        case "online":
                            LoggedUser.UserPresence.StatusText = "Online";
                            break;
                        case "away":
                            LoggedUser.UserPresence.StatusText = "Zaraz wracam";
                            break;
                        case "busy":
                            LoggedUser.UserPresence.StatusText = "Zajęty";
                            break;
                    }
                }
                else
                    LoggedUser.UserPresence.StatusText = obj.fields.message;
            }
        }

        private async Task GetUsers()
        {
            Users = new ObservableCollection<User>(await rocketChatRest.GetUsersList());
            foreach(var user in Users)
            {
                ChangeUserStatusText(new UserConnectionStatusNotification { fields = new UserConnectionStatusFields { userId = user.Id, status = user.UserPresence.Status, message = user.UserPresence.StatusText } });
            }    
        }

        private void ChangeUserStatusText(UserConnectionStatusNotification obj)
        {
            if (string.IsNullOrEmpty(obj.fields.message))
            {
                switch (obj.fields.status)
                {
                    case "offline":
                        Users.FirstOrDefault(x => x.Id == obj.fields.userId).UserPresence.StatusText = "Offline";
                        break;
                    case "online":
                        Users.FirstOrDefault(x => x.Id == obj.fields.userId).UserPresence.StatusText = "Online";
                        break;
                    case "away":
                        Users.FirstOrDefault(x => x.Id == obj.fields.userId).UserPresence.StatusText = "Zaraz wracam";
                        break;
                    case "busy":
                        Users.FirstOrDefault(x => x.Id == obj.fields.userId).UserPresence.StatusText = "Zajęty";
                        break;
                }
            }
            else
                Users.FirstOrDefault(x => x.Id == obj.fields.userId).UserPresence.StatusText = obj.fields.message;
        }
        #endregion
        #endregion

        #region event handlers
        private async void UserConnectionStatusChangedHandler(UserConnectionStatusNotification obj)
        {
            if(obj.fields.userId == LoggedUser.Id)
            {
                await Windows.ApplicationModel.Core.CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                {
                    LoggedUser.UserPresence.Status = obj.fields.status;
                    ChangeCurrentStatus(obj);
                });
                
            }
            else
            {
                await Windows.ApplicationModel.Core.CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                {
                    Users.FirstOrDefault(x => x.Id == obj.fields.userId).UserPresence.Status = obj.fields.status;
                    ChangeUserStatusText(obj);
                });
            }
        }

        private async void NewMessageHandler(NewMessageNotification obj)
        {
            var message = new Message(obj);
            message.User = Users.FirstOrDefault(x => x.Id == message.User.Id);
            if (SelectedChannel != null && SelectedChannel.Id == obj.fields.args[0].rid)
                await Windows.ApplicationModel.Core.CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                {
                    Messages.Add(message);
                });
            else
            {
                await Windows.ApplicationModel.Core.CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                {
                    if (Channels.Any(x => x.Id == message.RoomId))
                    {
                        Channels.FirstOrDefault(x => x.Id == message.RoomId).HasUnreadedMessages = true;
                        Channels.FirstOrDefault(x => x.Id == message.RoomId).ChannelFontWeight = FontWeights.Bold;
                        notificationService.ShowNewMessageToastNotification(message, "channel", Channels.FirstOrDefault(x => x.Id == message.RoomId).Name);
                    }
                    if (Discussions.Any(x => x.Id == message.RoomId))
                    {
                        Discussions.FirstOrDefault(x => x.Id == message.RoomId).HasUnreadedMessages = true;
                        Discussions.FirstOrDefault(x => x.Id == message.RoomId).ChannelFontWeight = FontWeights.Bold;
                        if (obj.fields.args[0].t == null || obj.fields.args[0].t != "discussion-created")
                            notificationService.ShowNewMessageToastNotification(message, "discussion", Discussions.FirstOrDefault(x => x.Id == message.RoomId).Name);
                    }
                    if (DirectConversations.Any(x => x.Id == message.RoomId))
                    {
                        DirectConversations.FirstOrDefault(x => x.Id == message.RoomId).HasUnreadedMessages = true;
                        DirectConversations.FirstOrDefault(x => x.Id == message.RoomId).ChannelFontWeight = FontWeights.Bold;
                        notificationService.ShowNewMessageToastNotification(message, "directed", DirectConversations.FirstOrDefault(x => x.Id == message.RoomId).Name);
                    }
                    if (PrivateGroups.Any(x => x.Id == message.RoomId))
                    {
                        PrivateGroups.FirstOrDefault(x => x.Id == message.RoomId).HasUnreadedMessages = true;
                        PrivateGroups.FirstOrDefault(x => x.Id == message.RoomId).ChannelFontWeight = FontWeights.Bold;
                    }
                });
            }
        }
        #endregion

        #region override
        public override async void OnNavigatedTo(NavigatedToEventArgs e, Dictionary<string, object> viewModelState)
        {
            base.OnNavigatedTo(e, viewModelState);
            LoggedUser = rocketChatRest.User;
            ChangeCurrentStatus(new UserConnectionStatusNotification { fields = new UserConnectionStatusFields { status = LoggedUser.UserPresence.Status, message = LoggedUser.UserPresence.StatusText } });
            await GetUsers();
            await GetRooms();
        }
        #endregion
    }
}
