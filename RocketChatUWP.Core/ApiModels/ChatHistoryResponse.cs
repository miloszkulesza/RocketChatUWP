namespace RocketChatUWP.Core.ApiModels
{
    public class ChatHistoryResponse
    {
        public ChatHistoryMessage[] messages { get; set; }
        public bool success { get; set; }
    }

    public class ChatHistoryMessage
    {
        public string _id { get; set; }
        public string rid { get; set; }
        public string msg { get; set; }
        public string ts { get; set; }
        public MessageAuthor u { get; set; }
        public bool groupable { get; set; }
        public string _updatedAt { get; set; }
    }

    public class MessageAuthor
    {
        public string _id { get; set; }
        public string username { get; set; }
    }
}
