using System.ComponentModel.DataAnnotations;
using MimeKit;

namespace EmailAPI.DTOs
{
    public class EventDTO
    {
        public string To { get; set; }
        public string Subject { get; set; } = null!;
        public string Content { get; set; } = null!;
    }
}
