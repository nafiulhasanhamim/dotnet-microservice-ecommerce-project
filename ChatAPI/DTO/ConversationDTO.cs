namespace ChatAPI.DTOs
{
    public class CreateConversationDTO
    {
        public string UserId { get; set; }  
        public string AdminId { get; set; }
    }
    public class ConversationDTO
    {
        public string Id { get; set; }
        public string UserId { get; set; }
        public string AdminId { get; set; }
        public string LastMessage { get; set; }
        public DateTime LastUpdated { get; set; }
        public int UnreadCount { get; set; }
    }

}