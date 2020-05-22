﻿using RocketChatUWP.Core.ApiModels;
using System;

namespace RocketChatUWP.Core.Models
{
    public class Room
    {
        public Room(RoomResponse roomResponse)
        {
            Id = roomResponse._id;
            Name = roomResponse.name;
            T = roomResponse.t;
            UpdatedAt = Convert.ToDateTime(roomResponse._updatedAt);
            Default = roomResponse.Default;
            RoomOwner = roomResponse.u;
        }

        public string Id { get; set; }
        public string Name { get; set; }
        public string T { get; set; }
        public DateTime UpdatedAt { get; set; }
        public bool Default { get; set; }
        public RoomOwner RoomOwner { get; set; }
    }
}
