using System.Data;

namespace Backend.Domain
{
    public class ChatMessage
    {
        public string? Username {get; set;}
        public string? Text {get; set;}
        public int GameId {get; set;}
        public DateTime SentAt {get; set;}
    }
}