using Microsoft.AspNetCore.SignalR;
using System.Text.Json;
using SignalRServer.Hubs;
using SignalRServer.Models;
using SignalRServer.Models.Game;

namespace SignalRServer.Expressions;

public class StartExpression : IExpression
{
    public string Interpret(string command, HubCallerContext context, IHubCallerClients clients, IGroupManager groups)
    {
        Facade facade = Facade.GetInstance(clients as IHubContext<PlayerHub>);
        AbstractGame game = facade.GetGameByConnection(context);
        if (game == null)
            return "Error: You are not in a game room.";

        if (game.IsStarted)
            return "Error: The game has already started.";

        string userName = game.Players[context.ConnectionId];

        facade.StartGame(game.RoomName, userName, clients, context.ConnectionId).Wait();

        return "Game started successfully.";
    }
}