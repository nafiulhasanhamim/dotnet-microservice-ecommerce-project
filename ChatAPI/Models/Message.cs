using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

public class Message
{
    [Key]
    public string Id { get; set; }  // Primary Key

    [ForeignKey("Conversation")]
    public string ConversationId { get; set; } 

    public string SenderId { get; set; } 
    public string ReceiverId { get; set; } 

    public string Content { get; set; }  

    public DateTime Timestamp { get; set; } 

    public bool IsRead { get; set; } = false; 

    public Conversation Conversation { get; set; }
}
