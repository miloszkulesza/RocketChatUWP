using Prism.Windows.Mvvm;
using RocketChatUWP.Core.Api;
using RocketChatUWP.Core.Models;
using System.Collections.ObjectModel;
using System.Linq;

namespace RocketChatUWP.ViewModels
{
    public class ChatViewModel : ViewModelBase
    {
        #region private members
        private readonly IRocketChatRestApi rocketChatRest;
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
        #endregion

        #region commands

        #endregion

        #region ctor
        public ChatViewModel(IRocketChatRestApi rocketChatRest)
        {
            this.rocketChatRest = rocketChatRest;
            RegisterCommands();
            GetRooms();
            LoggedUser = this.rocketChatRest.User;
        }
        #endregion

        #region private methods
        private void RegisterCommands()
        {
           
        }

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
        #endregion
    }
}
