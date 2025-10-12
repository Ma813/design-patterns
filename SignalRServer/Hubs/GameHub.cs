using Microsoft.AspNetCore.SignalR;
using SignalRServer.Models;
using SignalRServer.GameLogic;

namespace SignalRServer.Hubs;

public class GameHub : Hub
{
    private static readonly Dictionary<string, string> UserRooms = []; // Rooms Dictionary - key is connectionId, value is roomName
    private static readonly Dictionary<string, Game> Games = []; // Games Dictionary  key is roomName, value is Game object

    public async Task JoinRoom(string roomName, string userName)
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

        Game game;
        if (Games.TryGetValue(roomName, out Game? value))
        {
            game = value;
        }
        else
        {
            game = new Game();
            Games[roomName] = game;
        }

        if (game.isStarted)
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
    }

    public async Task StartGame(string roomName)
        => await GameHandler.StartGame(Games[roomName], Clients);

    public async Task DrawCard(string roomName, string userName)
        => await GameHandler.DrawCard(Games[roomName], userName, Clients);

    public async Task PlayCard(string roomName, string userName, BaseCard card)
        => await GameHandler.PlayCard(Games[roomName], userName, card, Clients, Context);
}