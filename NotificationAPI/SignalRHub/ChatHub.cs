using Microsoft.AspNetCore.SignalR;

namespace NotificationAPI.SignalRHub
{
    public class ChatHub : Hub
    {
        public override async Task OnConnectedAsync()
        {
            var user = Context.User;
            var userId = Context.UserIdentifier;
            if (user != null)
                await Groups.AddToGroupAsync(Context.ConnectionId, $"user:{userId}");
            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            var userId = Context.UserIdentifier;
            if (userId != null)
                await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"user:{userId}");
            await base.OnDisconnectedAsync(exception);
        }
    }
}
