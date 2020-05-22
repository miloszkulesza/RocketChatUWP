using Prism.Windows.Mvvm;
using RocketChatUWP.Core.Api;
using RocketChatUWP.Core.Models;
using System.Collections.ObjectModel;
using System.Linq;

namespace RocketChatUWP.ViewModels
{
    public class ChatViewModel : ViewModelBase
    {
        private readonly IRocketChatRestApi rocketChatRest;

        private ObservableCollection<Room> rooms;
        public ObservableCollection<Room> Rooms
        {
            get { return rooms; }
            set { SetProperty(ref rooms, value); }
        }

        public ChatViewModel(IRocketChatRestApi rocketChatRest)
        {
            this.rocketChatRest = rocketChatRest;
            GetRooms();
        }

        private async void GetRooms()
        {
            var rooms = await rocketChatRest.GetRooms();
            Rooms = new ObservableCollection<Room>(rooms.ToList());
        }
    }
}
