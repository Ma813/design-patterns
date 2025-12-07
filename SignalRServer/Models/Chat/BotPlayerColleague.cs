using System.Runtime.CompilerServices;
using SignalRServer.Models;
using SignalRServer.Models.Chat;

namespace SignalRServer.Model.Chat.Colleagues;

public class BotPlayerColleague : IChatColleague {
    private static readonly Logger logger = Logger.GetInstance();
    public string Username { get; }
    public string ConnectionId => $"bot--{Username}";
    private IChatMediator _mediator;

    public BotPlayerColleague(string username)
    {
        Username = username;
    }

    public void SetMediator(IChatMediator mediator)
    {
        _mediator = mediator;
    }

    public Task Receive(Message message)
    {
        logger.LogInfo($"[BOT {Username}] received message from {message.Sender}: {message.Text}");
        return Task.CompletedTask;
    }

    public async Task SendRandomMessage(string roomName)
    {
        var messages = new[] {
            "Good luck everyone!",
            "Let's see what happens...",
            "Watch out, here I come!"
        };
        var random = new Random();
        var text = messages[random.Next(messages.Length)];
        
        await _mediator.SendMessage(Username, text, roomName);
    }
}