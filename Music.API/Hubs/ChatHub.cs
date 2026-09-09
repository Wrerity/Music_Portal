using Microsoft.AspNetCore.SignalR;
using Music.bisLog.Services;

namespace Music.API.Hubs;

public class ChatHub : Hub
{
    private readonly ChatService _service;
    public ChatHub(ChatService service) => _service = service;

    public override async Task OnConnectedAsync()
    {
        var http = Context.GetHttpContext();
        var username = http?.Request.Query["username"].FirstOrDefault()
            ?? Context.User?.Identity?.Name
            ?? $"Guest_{Context.ConnectionId[..5]}";

        var user = await _service.RegisterOrUpdateAsync(username!, Context.ConnectionId);
        // История (последние 50 из общей комнаты + личные)
        var history = await _service.GetHistoryAsync(1, 50, Context.ConnectionId);
        await Clients.Caller.SendAsync("LoadHistory", history);
        await Clients.All.SendAsync("UpdateUsers", await _service.GetActiveUsersAsync());
        await base.OnConnectedAsync();
    }

    public override async Task OnDisconnectedAsync(Exception? ex)
    {
        await _service.SetOfflineAsync(Context.ConnectionId);
        await Clients.All.SendAsync("UpdateUsers", await _service.GetActiveUsersAsync());
        await base.OnDisconnectedAsync(ex);
    }

    // Общее сообщение всем
    public async Task SendMessage(string content)
    {
        var msg = await _service.SendAllAsync(Context.ConnectionId, content, 1);
        await Clients.All.SendAsync("ReceiveMessage", msg);
    }

    public async Task SendMessageToAll(string content) => await SendMessage(content);

    // Точка-точка
    public async Task SendPrivateMessage(string receiverConnectionId, string content)
    {
        var msg = await _service.SendPrivateAsync(Context.ConnectionId, receiverConnectionId, content);
        await Clients.Client(receiverConnectionId).SendAsync("ReceivePrivateMessage", msg);
        await Clients.Caller.SendAsync("ReceivePrivateMessage", msg);
    }
}
