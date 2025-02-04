using Microsoft.AspNetCore.SignalR;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

public class NotificationHub : Hub
{
    // Tracks which families each user belongs to
    private static readonly ConcurrentDictionary<string, HashSet<string>> UserFamilies = new();

    public override async Task OnConnectedAsync()
    {
        var userId = Context.UserIdentifier;
        if (string.IsNullOrEmpty(userId)) return;

        var familyIds = await GetUserFamiliesAsync(userId);

        // Store user-family mapping in memory
        UserFamilies[userId] = new HashSet<string>(familyIds);

        // Add the user to SignalR groups for each family
        foreach (var familyId in familyIds)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, familyId);
        }

        await base.OnConnectedAsync();
    }

    public override async Task OnDisconnectedAsync(Exception exception)
    {
        var userId = Context.UserIdentifier;
        if (!string.IsNullOrEmpty(userId))
        {
            UserFamilies.TryRemove(userId, out _);
        }

        await base.OnDisconnectedAsync(exception);
    }

    public async Task SendNotificationToFamilies(List<string> targetFamilyIds)
    {
        var usersToNotify = new Dictionary<string, HashSet<string>>();

        foreach (var (userId, familyIds) in UserFamilies)
        {
            var matchedFamilies = familyIds.Intersect(targetFamilyIds).ToList();
            if (matchedFamilies.Any())
            {
                if (!usersToNotify.ContainsKey(userId))
                    usersToNotify[userId] = new HashSet<string>();

                usersToNotify[userId].UnionWith(matchedFamilies);
            }
        }

        // Send a single notification per user
        foreach (var (userId, families) in usersToNotify)
        {
            await Clients.User(userId).SendAsync("ReceiveNotification", families.ToList());
        }
    }

    private Task<List<string>> GetUserFamiliesAsync(string userId)
    {
        // Replace with actual database lookup
        return Task.FromResult(new List<string> { "Family1", "Family2" });
    }
}
