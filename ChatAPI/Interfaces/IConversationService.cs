using System.Collections.Generic;
using System.Threading.Tasks;
using ChatAPI.DTO;
using ChatAPI.DTOs;

public interface IConversationService
{
    Task<IEnumerable<ConversationDTO>> GetAllConversationsAsync(string userId);
    Task<IEnumerable<MessageDTO>> GetConversationMessagesAsync(string conversationId, string userId);
    Task<UnreadMessageCountDTO> GetUnreadMessagesAsync(string userId);
    Task SendMessageAsync(CreateMessageDTO messageDTO, string senderId);
    Task<List<MessageDTO>> GetMessagesByUserAsync(string loggedInUserId, string targetUserId);
}
