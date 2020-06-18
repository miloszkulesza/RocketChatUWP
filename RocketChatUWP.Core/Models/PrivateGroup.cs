using RocketChatUWP.Core.ApiModels;
using System;
using System.Collections.Generic;
using System.Text;

namespace RocketChatUWP.Core.Models
{
    public class PrivateGroup : Room
    {
        public PrivateGroup(RoomResponse room) : base(room)
        {

        }
    }
}
