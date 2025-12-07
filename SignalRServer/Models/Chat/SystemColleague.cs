// Models/Chat/Colleagues/SystemColleague.cs
using Microsoft.AspNetCore.SignalR;
using SignalRServer.Hubs;

namespace SignalRServer.Models.Chat.Colleagues;

public class SystemColleague : IChatColleague
{
    public string Username => "SYSTEM";
    public string ConnectionId => "system";
    private IChatMediator _mediator;
    private readonly IHubContext<PlayerHub> _hubContext;
    private readonly string _roomName;

    public SystemColleague(IHubContext<PlayerHub> hubContext, string roomName)
    {
        _hubContext = hubContext;
        _roomName = roomName;
    }

    public void SetMediator(IChatMediator mediator)
    {
        _mediator = mediator;
    }

    public Task Receive(Message message)
    {
        // System doesn't receive messages
        return Task.CompletedTask;
    }

    public async Task BroadcastSystemMessage(string text)
    {
        await _mediator.SendMessage(Username, text, _roomName);
    }
}