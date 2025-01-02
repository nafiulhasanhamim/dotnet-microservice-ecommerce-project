using ChatAPI.DTO;
using ChatAPI.DTOs;
using ChatAPI.Extensions;
using ChatAPI.Services.Caching;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/conversations")]
[Authorize]
public class ConversationController : ControllerBase
{
    private readonly IConversationService _conversationService;
    private readonly IRedisCacheService _cache;


    public ConversationController(IConversationService conversationService, IRedisCacheService cache)
    {
        _conversationService = conversationService;
        _cache = cache;
    }

    [HttpGet]
    public async Task<IActionResult> GetAllConversations()
    {
        var userId = User.GetUserId();
        var cacheConversations = await _cache.GetDataAsync<IEnumerable<ConversationDTO>>($"conversation-{userId}");
        if (cacheConversations is not null)
        {
            return Ok(cacheConversations);
        }
        var conversations = await _conversationService.GetAllConversationsAsync(userId);
        _cache.SetData($"conversation-{userId}", conversations);
        return Ok(conversations);
    }

    [HttpGet("{conversationId}/messages")]
    public async Task<IActionResult> GetConversationMessages(string conversationId)
    {
        var userId = User.GetUserId();
        var messages = await _conversationService.GetConversationMessagesAsync(conversationId, userId);
        return Ok(messages);
    }

    [HttpGet("chat/{targetUserId}")]
    public async Task<IActionResult> GetChatMessages(string targetUserId)
    {
        var loggedInUserId = User.GetUserId(); // Assuming JWT token or session

        try
        {
            var cacheMessages = await _cache.GetDataAsync<IEnumerable<MessageDTO>>($"message-{loggedInUserId}{targetUserId}");
            if (cacheMessages is not null)
            {
                return Ok(cacheMessages);
            }
            var messages = await _conversationService.GetMessagesByUserAsync(loggedInUserId, targetUserId);
            _cache.SetData($"message-{loggedInUserId}{targetUserId}", messages);
            _cache.SetData($"message-{targetUserId}{loggedInUserId}", messages);
            return Ok(messages);
        }
        catch (Exception ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    [HttpGet("unread-count")]
    public async Task<IActionResult> GetUnreadMessageCount()
    {
        var userId = User.GetUserId();
        var unreadCount = await _conversationService.GetUnreadMessagesAsync(userId);
        return Ok(unreadCount);
    }

    [HttpPost("send-message")]
    public async Task<IActionResult> SendMessage([FromBody] CreateMessageDTO messageDTO)
    {
        var senderId = User.GetUserId();
        await _conversationService.SendMessageAsync(messageDTO, senderId);
        return Ok(new { Message = "Message sent successfully." });
    }
}
