using System;
using System.Collections.Generic;
using System.Text;

namespace RocketChatUWP.Core.Models
{
    public class ServerAddress
    {
        public string HttpAddress { get; set; } = string.Empty;
        public string WebsocketAddress { get; set; } = string.Empty;
    }
}
