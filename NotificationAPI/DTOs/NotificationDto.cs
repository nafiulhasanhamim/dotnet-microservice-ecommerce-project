using System.ComponentModel.DataAnnotations;

namespace NotificationAPI.DTOs
{
    public class NotificationReadDto
    {
        public string NotificationId { get; set; }
        public string UserId { get; set; }
        public string Entity { get; set; }
        public string EntityId { get; set; }
        public bool IsRead { get; set; } = false;
        public string Title { get; set; } = null!;
        public string Message { get; set; } = null!;
    }

    public class NotificationCreateDto
    {
        [Required(ErrorMessage = "UserId is required")]
        public List<string>? UserId { get; set; }

        [Required(ErrorMessage = "Entity is required")]

        public string? Entity { get; set; }

        [Required(ErrorMessage = "EntityId is required")]
        public string? EntityId { get; set; }

        [Required(ErrorMessage = "Title is required")]
        public string? Title { get; set; }

        [Required(ErrorMessage = "Message is required")]
        public string? Message { get; set; }
    }
}