using Microsoft.AspNetCore.SignalR;

namespace SignalRServer.Models.Chat.Colleagues;

public class HumanPlayerColleague : IChatColleague
{
    public string Username { get; }
    public string ConnectionId { get; }
    private readonly IClientProxy _client;
    private IChatMediator _mediator;

    public HumanPlayerColleague(string username, string connectionId, IClientProxy client)
    {
        Username = username;
        ConnectionId = connectionId;
        _client = client;
    }

    public void SetMediator(IChatMediator mediator)
    {
        _mediator = mediator;
    }

    public async Task Receive(Message message)
    {
        //System.Console.WriteLine("{0} received the message", Username);
        await message.SendMessageAsync(_client);
    }

    public async Task Send(string text, string roomName)
    {
        await _mediator.SendMessage(Username, text, roomName);
    }
}