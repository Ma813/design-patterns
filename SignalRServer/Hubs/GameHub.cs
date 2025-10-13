using Microsoft.AspNetCore.SignalR;
using SignalRServer.Models;
using SignalRServer.Helpers;

namespace SignalRServer.Hubs;

public class GameHub : Hub
{
    private static readonly Dictionary<string, string> UserRooms = []; // Rooms Dictionary - key is connectionId, value is roomName
    private static readonly Dictionary<string, AbstractGame> Games = []; // Games Dictionary  key is roomName, value is Game object

    readonly AbstractGameCreator gameCreator = new GameCreator();
    private static readonly Logger logger = Logger.GetInstance();

    public async Task JoinRoom(string roomName, string userName,
        string gameMode = "Classic",
        CardGeneratingMode cardGeneratingMode = CardGeneratingMode.Normal,
        StrategyType cardPlacementStrategy = StrategyType.Normal)
    {
        var connectionId = Context.ConnectionId;

        // Player is in the provided room
        if (UserRooms.TryGetValue(connectionId, out string? room) && room == roomName)
        {
            return;
        }

        // Remove user from other rooms
        if (UserRooms.TryGetValue(connectionId, out room))
        {
            await Groups.RemoveFromGroupAsync(connectionId, room);
        }

        AbstractGame game;
        if (Games.TryGetValue(roomName, out AbstractGame? value))
        {
            game = value;
        }
        else
        {
            game = gameCreator.CreateGame(gameMode, cardGeneratingMode, cardPlacementStrategy);
            Games[roomName] = game;
        }

        if (game.IsStarted)
        {
            return; // TODO: Should return an error
        }

        game.Players[Context.ConnectionId] = userName;

        // Add user to new room
        UserRooms[connectionId] = roomName;
        await Groups.AddToGroupAsync(connectionId, roomName);

        // Send only the array of usernames to the group, not the full dictionary
        var usernames = game.Players.Values.ToArray();
        await Clients.Group(roomName).SendAsync("UserJoined", usernames);

        logger.LogInfo($"{userName} joined room {roomName} (connectionId: {connectionId})");
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        var connectionId = Context.ConnectionId;
        if (UserRooms.TryGetValue(connectionId, out var roomName))
        {
            await Clients.Group(roomName).SendAsync("UserLeft", $"{connectionId} left the room");
            UserRooms.Remove(connectionId);
        }

        await base.OnDisconnectedAsync(exception);

        logger.LogInfo($"Connection {connectionId} disconnected");
    }

    public async Task StartGame(string roomName)
    {
        AbstractGame game = Games[roomName];
        game.Start();

        foreach (var player in game.Players)
        {
            GameDto gameDto = new(game, player.Value);
            await Clients.Client(player.Key).SendAsync("GameStarted", gameDto);
        }
    }

    public async Task DrawCard(string roomName, string userName)
    {
        AbstractGame game = Games[roomName];
        game.DrawCard(userName);
        await NotifyPlayers(game);
    }

    public async Task PlayCard(string roomName, string userName, BaseCard card)
    {
        AbstractGame game = Games[roomName];
        string result = game.PlayCard(userName, card);

        if (result == "WIN")
        {
            await Clients.Group(roomName).SendAsync("GameEnded", $"{userName} has won the game!");
            return;
        }
        if (result != "OK")
        {
            await Clients.Caller.SendAsync("Error", result);
            return;
        }

        await NotifyPlayers(game);
    }

    private async Task NotifyPlayers(AbstractGame game)
    {
        foreach (var player in game.Players)
        {
            GameDto gameForSending = new(game, player.Value);
            await Clients.Client(player.Key).SendAsync("GameStatus", gameForSending);
        }
    }
}