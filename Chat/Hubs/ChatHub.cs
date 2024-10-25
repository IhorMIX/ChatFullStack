using Chat.Models;
using Microsoft.AspNetCore.SignalR;

namespace Chat.Hubs;

public interface IChatClient
{
    public Task ReceiveMessage(string userName, string message);
}
public class ChatHub : Hub<IChatClient>
{
    public async Task JoinChat(UserConnection connection)
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, connection.ChatRoom);

        await Clients.Group(connection.ChatRoom)
            .ReceiveMessage("", "has been joined to chat");
    }
}