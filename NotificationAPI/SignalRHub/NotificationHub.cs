using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SignalR;

namespace NotificationAPI.SignalRHub
{
    public class NotificationHub : Hub
    {   
        public override async Task OnConnectedAsync()
        {
            var user = Context.User;
            var userId = Context.UserIdentifier;
            if (user != null && user.IsInRole("Admin")) 
            {
                await Groups.AddToGroupAsync(Context.ConnectionId, "admin");
            }
            else if (userId != null)
            {
                await Groups.AddToGroupAsync(Context.ConnectionId, $"user:{userId}");
            }

            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            var userId = Context.UserIdentifier;

            if (userId != null)
            {
                await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"user:{userId}");
            }
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, "admin");

            await base.OnDisconnectedAsync(exception);
        }

    }
}
