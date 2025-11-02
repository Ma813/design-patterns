using System.Runtime.InteropServices;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using SignalRServer.Models;
using SignalRServer.Helpers;

namespace SignalRServer.Hubs;

public class GameHub : Hub
{
    private readonly Facade facade;
    public GameHub(Facade facade)
    {
        this.facade = facade;
    }

    public async Task JoinRoom(string roomName, string userName, int botAmount = 0,
        GameType gameMode = GameType.Classic,
        CardGeneratingMode cardGeneratingMode = CardGeneratingMode.Normal,
        StrategyType cardPlacementStrategy = StrategyType.Normal)
    {
        await facade.JoinRoom(roomName, userName, Clients, Context, Groups, botAmount, gameMode, cardGeneratingMode, cardPlacementStrategy);
    }

    // TODO add normal error handling
    // public override async Task OnDisconnectedAsync(Exception? exception)
    // {
    //     var connectionId = Context.ConnectionId;
    //     if (UserRooms.TryGetValue(connectionId, out var roomName))
    //     {
    //         await Clients.Group(roomName).SendAsync("UserLeft", $"{connectionId} left the room");
    //         UserRooms.Remove(connectionId);
    //     }

    //     await base.OnDisconnectedAsync(exception);

    //     logger.LogInfo($"Connection {connectionId} disconnected");
    // }

    public async Task StartGame(string roomName)
    {
        await facade.StartGame(roomName, Clients);
    }

    public async Task DrawCard(string roomName, string userName)
    {
        await facade.DrawCard(roomName, userName);
    }

    public async Task PlayCard(string roomName, string userName, CardDto cardDto)
    {
        await facade.PlayCard(roomName, userName, cardDto, Clients);
    }


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
}