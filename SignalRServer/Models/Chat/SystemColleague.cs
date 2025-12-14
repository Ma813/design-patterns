// Models/Chat/Colleagues/SystemColleague.cs
using Microsoft.AspNetCore.SignalR;
using SignalRServer.Hubs;

namespace SignalRServer.Models.Chat.Colleagues;

public class SystemColleague : IChatColleague
{
    private static readonly Logger logger = Logger.GetInstance();
    
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
        // System can respond to special commands
        if (message.Text.StartsWith("/"))
        {
            _ = HandleCommand(message.Text, message.Sender);
        }
        return Task.CompletedTask;
    }

    private async Task HandleCommand(string command, string sender)
    {
        string response = command.ToLower().Trim() switch
        {
            "/help" => "Available commands: /help, /rules, /players",
            "/rules" => "Match cards by color or number. First to empty their hand wins!",
            "/players" => "Use the player list on the left to see who's in the game.",
            _ => $"Unknown command: {command}. Type /help for available commands."
        };
        
        logger.LogInfo($"[SystemColleague] Responding to command '{command}' from {sender}");
        await _mediator.SendMessage(Username, response, _roomName);
    }

    public async Task BroadcastSystemMessage(string text)
    {
        logger.LogInfo($"[SystemColleague] Broadcasting: {text}");
        await _mediator.SendMessage(Username, text, _roomName);
    }

    // Game event announcements
    public async Task AnnounceGameStarted()
    {
        await BroadcastSystemMessage("🚀 The game has started! Good luck everyone!");
    }

    public async Task AnnouncePlayerJoined(string playerName)
    {
        await BroadcastSystemMessage($"🎮 {playerName} has joined the game!");
    }

    public async Task AnnouncePlayerLeft(string playerName)
    {
        await BroadcastSystemMessage($"👋 {playerName} has left the game.");
    }
    public async Task AnnounceWinner(string playerName)
    {
        await BroadcastSystemMessage($"🏆 Congratulations! {playerName} has won the game!");
    }

    public async Task AnnounceUnoCall(string playerName)
    {
        await BroadcastSystemMessage($"⚠️ {playerName} has only ONE card left!");
    }
}