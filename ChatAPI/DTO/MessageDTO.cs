using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ChatAPI.DTO
{
    public class CreateMessageDTO
    {
        public string ReceiverId { get; set; }
        public string Content { get; set; }
    }
    public class MessageDTO
    {
        public string Id { get; set; }
        public string ConversationId { get; set; }
        public string SenderId { get; set; }
        public string ReceiverId { get; set; }
        public string Content { get; set; }
        public DateTime Timestamp { get; set; }
        public bool IsRead { get; set; }
    }
    public class UnreadMessageCountDTO
    {
        public int ConversationId { get; set; }
        public int UnreadCount { get; set; }
    }

}