namespace NotificationAPI.Models
{
    public class Notification
    {
        public string NotificationId { get; set; }
        public string UserId { get; set; }
        public string Entity { get; set; }
        public string EntityId { get; set; }
        public bool IsRead { get; set; } = false;
        public string Title { get; set; } = null!;
        public string Message { get; set; } = null!;
        public DateTime CreatedAt { get; set; }

    }
}