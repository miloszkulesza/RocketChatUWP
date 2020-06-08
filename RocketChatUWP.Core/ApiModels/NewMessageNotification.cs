using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace RocketChatUWP.Core.ApiModels
{
    public class NewMessageNotification
    {
        public string msg { get; set; }
        public string collection { get; set; }
        public string id { get; set; }
        public NewMessageNotificationFields fields { get; set; }
    }

    public class NewMessageNotificationFields
    {
        public string eventName { get; set; }
        public NewMessageArgs[] args { get; set; }
    }

    public class NewMessageArgs
    {
        public string _id { get; set; }
        public string rid { get; set; }
        public string msg { get; set; }
        public NewMessageTimestamp ts { get; set; }
        public NewMessageUser u { get; set; }
        public object[] mentions { get; set; }
        public object[] channels { get; set; }
        public NewMessageTimestamp _updatedAt { get; set; }
        public bool roomParticipant { get; set; }
        public string roomType { get; set; }
        public string roomName { get; set; }
        public string t { get; set; }
    }

    public class NewMessageTimestamp
    {
        [JsonProperty("$date")]
        public string date { get; set; }
    }

    public class NewMessageUser
    {
        public string _id { get; set; }
        public string username { get; set; }
    }
}
