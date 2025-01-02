namespace NotificationAPI.DTOs
{
    public class EventDto
    {
        public List<string>? UserId { get; set; }
        public string Entity { get; set; } = null!;
        public string EntityId { get; set; } = null!;
        public string Title { get; set; } = null!;
        public string Message { get; set; } = null!;
        public string Whom { get; set; } = null!;

    }
    public class ChatEventDto
    {
        public List<string>? UserId { get; set; }
        public string Message { get; set; } = null!;
    }
}