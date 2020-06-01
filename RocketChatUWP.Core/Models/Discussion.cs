using RocketChatUWP.Core.ApiModels;

namespace RocketChatUWP.Core.Models
{
    public class Discussion : Room
    {
        public string Topic { get; set; }
        public string ParentId { get; set; }

        public Discussion(RoomResponse room): base(room)
        {
            Topic = room.topic;
            Name = room.fname;
            ParentId = room.prid;
        }
    }
}
