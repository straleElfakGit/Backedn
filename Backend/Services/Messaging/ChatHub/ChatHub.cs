using Microsoft.AspNetCore.SignalR;
using Backend.Domain;

namespace Srbopoly.Services.Messaging.ChatHub;

public class ChatHub : Hub
{
    public async Task JoinGameGroup(int gameId)
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, $"game-{gameId}");
    }

    public async Task LeaveGameGroup(int gameId)
    {
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"game-{gameId}");
    }
}