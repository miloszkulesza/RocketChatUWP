using Windows.ApplicationModel.Chat;

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
        public string t { get; set; }
        public MessageAttachment[] attachments { get; set; }
        public MessageFileInfo file { get; set; }
    }

    public class MessageAuthor
    {
        public string _id { get; set; }
        public string username { get; set; }
    }

    public class MessageAttachment
    {
        public string title { get; set; }
        public string type { get; set; }
        public string description { get; set; }
        public string title_link { get; set; }
        public bool title_link_download { get; set; }
        public string image_url { get; set; }
        public string image_type { get; set; }
        public int image_size { get; set; }
        public MessageImageDimensions image_dimensions { get; set; }
        public string image_preview { get; set; }
    }

    public class MessageFileInfo
    {
        public string _id { get; set; }
        public string name { get; set; }
        public string type { get; set; }
    }

    public class MessageImageDimensions
    {
        public int width { get; set; }
        public int height { get; set; }
    }
}
