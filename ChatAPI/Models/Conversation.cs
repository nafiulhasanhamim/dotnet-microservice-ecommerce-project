using System.ComponentModel.DataAnnotations;

public class Conversation
{
    [Key]
    public string Id { get; set; } 

    public string UserId { get; set; }  
    public string AdminId { get; set; } 

    public string LastMessage { get; set; } 

    public DateTime LastUpdated { get; set; }

    public int UnreadCount { get; set; } 
}
