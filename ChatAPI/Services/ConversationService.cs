using AutoMapper;
using ChatAPI;
using ChatAPI.DTO;
using ChatAPI.DTOs;
using ChatAPI.Interfaces;
using ChatAPI.RabbitMQ;
using ChatAPI.Services.Caching;
using Microsoft.EntityFrameworkCore;

public class ConversationService : IConversationService
{
    private readonly AppDbContext _context;
    private readonly IMapper _mapper;
    private readonly IUserService _userService;
    private readonly IRedisCacheService _cache;
    private readonly IRabbmitMQCartMessageSender _messagebus;


    public ConversationService(AppDbContext context, IMapper mapper, IUserService userService, IRedisCacheService cache,
    IRabbmitMQCartMessageSender messagebus)
    {
        _context = context;
        _mapper = mapper;
        _userService = userService;
        _cache = cache;
        _messagebus = messagebus;
    }

    public async Task<IEnumerable<ConversationDTO>> GetAllConversationsAsync(string userId)
    {
        var conversations = await _context.Conversations
            .Where(c => c.UserId == userId || c.AdminId == userId)
            .OrderByDescending(c => c.LastUpdated)
            .ToListAsync();

        return _mapper.Map<IEnumerable<ConversationDTO>>(conversations);
    }

    public async Task<IEnumerable<MessageDTO>> GetConversationMessagesAsync(string conversationId, string userId)
    {
        var conversation = await _context.Conversations.FindAsync(conversationId);

        if (conversation == null || (conversation.UserId != userId && conversation.AdminId != userId))
        {
            throw new UnauthorizedAccessException("You are not authorized to view this conversation.");
        }

        var messages = await _context.Messages
            .Where(m => m.ConversationId == conversationId)
            .OrderBy(m => m.Timestamp)
            .ToListAsync();

        // Mark unread messages as read if the current user is the receiver
        foreach (var message in messages.Where(m => m.ReceiverId == userId && !m.IsRead))
        {
            message.IsRead = true;
        }

        await _context.SaveChangesAsync();

        return _mapper.Map<IEnumerable<MessageDTO>>(messages);
    }


    public async Task<List<MessageDTO>> GetMessagesByUserAsync(string loggedInUserId, string targetUserId)
    {
        var sender = await _userService.GetUser(loggedInUserId);
        var receiver = await _userService.GetUser(targetUserId);
        Console.WriteLine(sender.UserId);
        Console.WriteLine(receiver.UserId);
        if (sender == null || receiver == null)
        {
            throw new InvalidOperationException("Invalid sender or receiver.");
        }

        string? userId = null;
        string? adminId = null;

        if (sender.Roles[0] == "User" && receiver.Roles[0] == "Admin")
        {
            userId = loggedInUserId;
            adminId = targetUserId;
        }
        else if (sender.Roles[0] == "Admin" && receiver.Roles[0] == "User")
        {
            userId = targetUserId;
            adminId = loggedInUserId;
        }
        else
        {
            throw new InvalidOperationException("Invalid conversation roles.");
        }

        var conversation = await _context.Conversations
            .FirstOrDefaultAsync(c => c.UserId == userId && c.AdminId == adminId);

        if (conversation == null)
        {
            return new List<MessageDTO>();
        }

        var messages = await _context.Messages
            .Where(m => m.ConversationId == conversation.Id)
            .OrderBy(m => m.Timestamp)
            .Select(m => new MessageDTO
            {
                Id = m.Id,
                ConversationId = conversation.Id,
                SenderId = m.SenderId,
                ReceiverId = m.ReceiverId,
                Content = m.Content,
                Timestamp = m.Timestamp,
                IsRead = m.IsRead
            })
            .ToListAsync();

        foreach (var message in messages.Where(m => m.ReceiverId == userId && !m.IsRead))
        {
            message.IsRead = true;
        }
        _messagebus.SendMessage(new { UserId = new List<string> { $"{loggedInUserId}", }, Message = "Update Numbering" }, "chatSenderNotification", "queue");
        return messages;
    }

    public async Task<UnreadMessageCountDTO> GetUnreadMessagesAsync(string userId)
    {
        var unreadCount = await _context.Messages
            .CountAsync(m => m.ReceiverId == userId && !m.IsRead);

        return new UnreadMessageCountDTO
        {
            UnreadCount = unreadCount
        };
    }

    public async Task SendMessageAsync(CreateMessageDTO messageDTO, string senderId)
    {
        var sender = await _userService.GetUser(senderId);
        var receiver = await _userService.GetUser(messageDTO.ReceiverId);

        if (sender == null || receiver == null)
        {
            throw new InvalidOperationException("Invalid sender or receiver.");
        }

        if (sender.Roles[0] == "User" && receiver.Roles[0] != "Admin")
        {
            throw new InvalidOperationException("Users can only send messages to Admins.");
        }
        if (sender.Roles[0] == "Admin" && receiver.Roles[0] != "User")
        {
            throw new InvalidOperationException("Admins can only send messages to Users.");
        }

        var conversation = await _context.Conversations
            .FirstOrDefaultAsync(c =>
                (c.UserId == senderId && c.AdminId == messageDTO.ReceiverId) ||
                (c.UserId == messageDTO.ReceiverId && c.AdminId == senderId));

        if (conversation == null)
        {
            conversation = new Conversation
            {
                Id = Guid.NewGuid().ToString(),
                UserId = sender.Roles[0] == "User" ? senderId : messageDTO.ReceiverId,
                AdminId = sender.Roles[0] == "Admin" ? senderId : messageDTO.ReceiverId,
                LastMessage = messageDTO.Content,
                LastUpdated = DateTime.UtcNow,
                UnreadCount = 1
            };

            await _context.Conversations.AddAsync(conversation);
            await _context.SaveChangesAsync();
        }
        else
        {
            conversation.LastMessage = messageDTO.Content;
            conversation.LastUpdated = DateTime.UtcNow;
            conversation.UnreadCount += 1;
        }
        var message = new Message
        {
            Id = Guid.NewGuid().ToString(),
            ConversationId = conversation.Id,
            SenderId = senderId,
            ReceiverId = messageDTO.ReceiverId,
            Content = messageDTO.Content,
            Timestamp = DateTime.UtcNow,
            IsRead = false
        };
        _cache.RemoveData($"conversation-{sender.UserId}");
        _cache.RemoveData($"conversation-{receiver.UserId}");
        _cache.RemoveData($"message-{sender.UserId}{receiver.UserId}");
        _cache.RemoveData($"message-{receiver.UserId}{sender.UserId}");
        _messagebus.SendMessage(new { UserId = new List<string> { $"{receiver.UserId}", }, Message = "New Messages" }, "chatReceiverNotification", "queue");
        await _context.Messages.AddAsync(message);
        await _context.SaveChangesAsync();

    }
}

