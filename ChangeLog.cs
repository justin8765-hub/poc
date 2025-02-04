using Microsoft.AspNetCore.SignalR;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Security.Claims;
using System.Linq;
using System.Threading.Tasks;

public class ConnectedUser
{
    public string UserId { get; set; } = string.Empty;
    public string ConnectionId { get; set; } = string.Empty;
    public ClaimsIdentity? ClaimsIdentity { get; set; }
    public HashSet<string> Families { get; set; } = new();
}

public class NotificationHub : Hub
{
    // Store connected users (Thread-safe)
    private static readonly ConcurrentDictionary<string, ConnectedUser> ConnectedUsers = new();

    public override async Task OnConnectedAsync()
    {
        var userId = GetUserId();
        var connectionId = Context.ConnectionId;
        var claimsIdentity = Context.User?.Identity as ClaimsIdentity;
        var families = await GetUserFamiliesAsync(userId);

        if (!string.IsNullOrEmpty(userId))
        {
            // Store user details
            var user = new ConnectedUser
            {
                UserId = userId,
                ConnectionId = connectionId,
                ClaimsIdentity = claimsIdentity,
                Families = new HashSet<string>(families)
            };

            ConnectedUsers[userId] = user;

            // Join SignalR groups for families
            foreach (var familyId in families)
            {
                await Groups.AddToGroupAsync(connectionId, familyId);
            }
        }

        await base.OnConnectedAsync();
    }

    public override async Task OnDisconnectedAsync(Exception exception)
    {
        var userId = GetUserId();
        if (!string.IsNullOrEmpty(userId) && ConnectedUsers.TryRemove(userId, out var user))
        {
            // Remove user from SignalR groups
            foreach (var familyId in user.Families)
            {
                await Groups.RemoveFromGroupAsync(user.ConnectionId, familyId);
            }
        }

        await base.OnDisconnectedAsync(exception);
    }

    public static List<ConnectedUser> GetConnectedUsers()
    {
        return ConnectedUsers.Values.ToList();
    }

    private string GetUserId()
    {
        return Context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? string.Empty;
    }

    private async Task<List<string>> GetUserFamiliesAsync(string userId)
    {
        // Replace with actual database lookup
        await Task.Delay(100);
        return new List<string> { "Family1", "Family2" }; // Example families
    }
}
