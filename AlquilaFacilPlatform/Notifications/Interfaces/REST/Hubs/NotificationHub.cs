using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace AlquilaFacilPlatform.Notifications.Interfaces.REST.Hubs;

[Authorize]
public class NotificationHub : Hub
{
    public override async Task OnConnectedAsync()
    {
        var userId = Context.UserIdentifier;
        if (!string.IsNullOrEmpty(userId))
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, $"notifications_{userId}");
        }
        await base.OnConnectedAsync();
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        var userId = Context.UserIdentifier;
        if (!string.IsNullOrEmpty(userId))
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"notifications_{userId}");
        }
        await base.OnDisconnectedAsync(exception);
    }

    public async Task MarkAsRead(int notificationId)
    {
        // This could be extended to update the notification status in the database
        await Clients.Caller.SendAsync("NotificationMarkedAsRead", notificationId);
    }
}
