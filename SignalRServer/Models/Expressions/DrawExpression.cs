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

        string userName = game.Players[context.ConnectionId];

        string result = facade.DrawCard(game.RoomName, userName, clients).Result;
        if (result != "OK")
            return result;

        foreach (var player in game.Players)
        {
            if (player.Key != context.ConnectionId)
                clients.Client(player.Key).SendAsync("SystemMessage", $"{userName} has drawn a card.").Wait();
            GameForSending gameForSeding = new GameForSending(game, player.Value);
            clients.Client(player.Key).SendAsync("SystemMessage", gameForSeding.ToConsoleString()).Wait();

        }

        return "Drew a card.";
    }
}