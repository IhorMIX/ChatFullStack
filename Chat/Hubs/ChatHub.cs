using Chat.Models;
using Microsoft.AspNetCore.SignalR;

namespace Chat.Hubs;

public class ChatHub : Hub
{
    public async Task JoinChat(UserConnection connection)
    {
        
    }
}