using Microsoft.AspNetCore.SignalR;
using SignalRServer.Models;

namespace SignalRServer.Hubs;


//TODO rename to PlayerHub or PlayerClientHub
public class GameHub : Hub
{
    private static readonly Logger logger = Logger.GetInstance();

    private readonly Facade facade;

    public GameHub(Facade facade)
    {
        // Dependency Injection
        this.facade = facade;
    }

    public async Task JoinRoom(string roomName, string userName, int botAmount = 0, string gameMode = "Classic", string cardPlacementStrategy = "UnoPlacementStrategy")
    {

        logger.LogInfo($"User {userName} is joining room {roomName} with {botAmount} bots, game mode: {gameMode}, card placement strategy: {cardPlacementStrategy}");
        await facade.JoinRoom(roomName, userName, Clients, Context, Groups, botAmount, gameMode, cardPlacementStrategy);
    }

    public async Task StartGame(string roomName, string userName)
    {
        logger.LogInfo($"User {userName} is starting game in room {roomName}");
        await facade.StartGame(roomName, userName, Clients);
    }

    public async Task DrawCard(string roomName, string userName)
    {
        await facade.DrawCard(roomName, userName, Clients);
    }

    public async Task<bool> PlayCard(string roomName, string userName, UnoCard card)
    {
        bool result = await facade.PlayCard(roomName, userName, card, Clients);
        return result;
    }

    public async Task UndoCard(string roomName, string userName)
    {
        await facade.UndoCard(roomName, userName, Clients);
    }


    //TODO add normal disconnection handling
    // public override async Task OnDisconnectedAsync(Exception? exception)
    // {
    //     var connectionId = Context.ConnectionId;
    //     if (UserRooms.TryGetValue(connectionId, out var roomName))
    //     {
    //         await Clients.Group(roomName).SendAsync("UserLeft", $"{connectionId} left the room");
    //         UserRooms.Remove(connectionId);
    //     }

    //     await base.OnDisconnectedAsync(exception);
    // }

    public async Task AnnoyPlayers(string roomName, string message)
    {
        await facade.AnnoyPlayers(roomName, message, Clients, Context);
    }

    public async Task AnnoyPlayer(string roomName, string player, string message)
    {
        await facade.AnnoyPlayer(roomName, player, message, Clients, Context);
    }

    public async Task ToggleMutePlayer(string roomname, string player)
    {
        await facade.ToggleMutePlayer(roomname, player, Clients, Context);
    }

    public async Task SendMessage(string roomName, string sender, string text)
    {
        await facade.SendTextMessage(roomName, sender, text);
    }

    public async Task NextPlayer(string roomName, string actionType)
    {
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
