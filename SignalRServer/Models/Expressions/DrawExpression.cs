using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using SignalRServer.Card;
using SignalRServer.Hubs;
using SignalRServer.Models;
using SignalRServer.Models.Game;

namespace SignalRServer.Expressions;

public class DrawExpression : IExpression
{
    public string Interpret(string command, HubCallerContext context, IHubCallerClients clients, IGroupManager groups)
    {
        Facade facade = Facade.GetInstance(clients as IHubContext<PlayerHub>);
        AbstractGame game = facade.GetGameByConnection(context);
        if (game == null)
            return "Error: You are not in a game room.";

        if (game.IsStarted == false)
            return "Error: The game has not started yet.";

        string username = game.Players[context.ConnectionId];


        int index = game.Players.Keys.ToList().IndexOf(context.ConnectionId);

        if (game.CurrentPlayerIndex != index)
            return "Error: It is not your turn.";

        string userName = game.Players[context.ConnectionId];

        string result = facade.DrawCard(game.RoomName, userName, clients, context.ConnectionId).Result;
        if (result != "OK")
            return result;


        return "Drew a card.";
    }
}