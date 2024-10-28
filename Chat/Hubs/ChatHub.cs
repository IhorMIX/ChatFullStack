using System.Text.Json;
using Chat.Models;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Caching.Distributed;

namespace Chat.Hubs;

public interface IChatClient
{
    public Task ReceiveMessage(string userName, string message);
}

public class ChatHub(IDistributedCache cache) : Hub<IChatClient>
{
    private readonly IDistributedCache _cache = cache;

    public async Task JoinChat(UserConnection connection)
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, connection.ChatRoom);

        var stringConnection = JsonSerializer.Serialize(connection);

        await _cache.SetStringAsync(Context.ConnectionId,stringConnection);
        
        await Clients.Group(connection.ChatRoom)
            .ReceiveMessage("", $"{connection.Username} has been joined to chat");
    }

    public async Task SendMessage(string message)
    {
        var stringConnection = await _cache.GetAsync(Context.ConnectionId);

        var connection = JsonSerializer.Deserialize<UserConnection>(stringConnection);

        if (connection is not null)
        {
            await Clients.Group(connection.ChatRoom)
                .ReceiveMessage(connection.Username, message);
        }
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        var stringConnection = await _cache.GetAsync(Context.ConnectionId);
        var connection = JsonSerializer.Deserialize<UserConnection>(stringConnection);

        if (connection is not null)
        {
            await _cache.RemoveAsync(Context.ConnectionId);
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, connection.ChatRoom);
            
            await Clients.Group(connection.ChatRoom)
                .ReceiveMessage("", $"{connection.Username} left from chat");
        }
        
        await base.OnDisconnectedAsync(exception);
    }
}