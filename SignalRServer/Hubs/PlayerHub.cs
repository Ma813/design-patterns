using Microsoft.AspNetCore.SignalR;
using SignalRServer.Models;

namespace SignalRServer.Hubs;

public class PlayerHub : Hub
{
    private static readonly Logger logger = Logger.GetInstance();

    private readonly Facade facade;

    public PlayerHub(Facade facade)
    {
        this.facade = facade;
    }

    public async Task JoinRoom(string roomName, string userName, int botAmount = 0, string gameMode = "Classic", string cardPlacementStrategy = "UnoPlacementStrategy", string theme = "Classic")
    {

        logger.LogInfo($"User {userName} is joining room {roomName} with {botAmount} bots, game mode: {gameMode}, card placement strategy: {cardPlacementStrategy}, theme: {theme}");
        await facade.JoinRoom(roomName, userName, Clients, Context, Groups, botAmount, gameMode, cardPlacementStrategy, theme);
    }

    public async Task StartGame(string roomName, string userName)
    {
        logger.LogInfo($"User {userName} is starting game in room {roomName}");
        await facade.StartGame(roomName, userName, Clients);
    }

    public async Task DrawCard(string roomName, string userName)
    {
        logger.LogInfo($"User {userName} is drawing a card in room {roomName}");
        logger.LogInfo($"User {userName} is drawing a card in room {roomName}");
        await facade.DrawCard(roomName, userName, Clients);
    }

    public async Task<bool> PlayCard(string roomName, string userName, UnoCard card)
    {
        logger.LogInfo($"User {userName} is playing a card in room {roomName}");
        logger.LogInfo($"User {userName} is playing a card in room {roomName}");
        bool result = await facade.PlayCard(roomName, userName, card, Clients);
        return result;
    }

    public async Task UndoCard(string roomName, string userName)
    {
        logger.LogInfo($"User {userName} is undoing a card in room {roomName}");
        logger.LogInfo($"User {userName} is undoing a card in room {roomName}");
        await facade.UndoCard(roomName, userName, Clients);
    }

    public async Task AnnoyPlayers(string roomName, string message)
    {
        logger.LogInfo($"All players annoyed in room {roomName}");
        logger.LogInfo($"All players annoyed in room {roomName}");
        await facade.AnnoyPlayers(roomName, message, Clients, Context);
    }

    public async Task AnnoyPlayer(string roomName, string player, string message)
    {
        logger.LogInfo($"Player {player} annoyed in room {roomName}");
        logger.LogInfo($"Player {player} annoyed in room {roomName}");
        await facade.AnnoyPlayer(roomName, player, message, Clients, Context);
    }

    public async Task ToggleMutePlayer(string roomname, string player)
    {
        logger.LogInfo($"Player {player} muted/unmuted in room {roomname}");
        logger.LogInfo($"Player {player} muted/unmuted in room {roomname}");
        await facade.ToggleMutePlayer(roomname, player, Clients, Context);
    }

    public async Task SendMessage(string roomName, string sender, string text)
    {
        logger.LogInfo($"User {sender} sent a message in room {roomName}: {text}");
        logger.LogInfo($"User {sender} sent a message in room {roomName}: {text}");
        await facade.SendTextMessage(roomName, sender, text);
    }

    public async Task NextPlayer(string roomName, string actionType)
    {
        logger.LogInfo($"Next player turn in room {roomName}");
       
        logger.LogInfo($"Next player turn in room {roomName}");
        await facade.NextPlayer(roomName, actionType, Clients);
    }
public async Task JoinRoomThroughDirector(string roomName, string userName, string builderType)
    {
        
        await facade.JoinRoomThroughDirector(roomName, userName, builderType, Clients, Context, Groups);
}
    // private async Task notifyPlayers(AbstractGame game)
    // {
    //     foreach (var player in game.Players)
    //     {
    //         GameForSending gameForSending = new GameForSending(game, player.Value);
    //         await Clients.Client(player.Key).SendAsync("GameStatus", gameForSending);
    //     }
    // }

    // // Add this method to get current connection status
    // public string GetConnectionId() => Context.ConnectionId;
        
    
}
