using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using SignalRServer.Card;
using SignalRServer.Hubs;
using SignalRServer.Models;
using SignalRServer.Models.Game;

namespace SignalRServer.Expressions;

public class PlayExpression : IExpression
{
    public string Interpret(string command, HubCallerContext context, IHubCallerClients clients, IGroupManager groups)
    {
        // Command format: "play cardIndex"
        string[] parts = command.Split(' ', 2, StringSplitOptions.RemoveEmptyEntries);

        if (parts.Length < 2 || !int.TryParse(parts[1], out int cardIndex))
            return "Error: Invalid command format. Usage: play <cardIndex>";

        Facade facade = Facade.GetInstance(clients as IHubContext<PlayerHub>);
        AbstractGame game = facade.GetGameByConnection(context);
        if (game == null)
            return "Error: You are not in a game room.";

        string userName = game.Players[context.ConnectionId];


        string result = facade.PlayCard(game.RoomName, userName, cardIndex, clients).Result;
        if (result != "OK")
            return result;
        if (result == "WIN")
        {
            foreach (var player in game.Players)
            {
                if (player.Key != context.ConnectionId)
                    clients.Client(player.Key).SendAsync("GameEnded", $"{userName} has won the game!").Wait();

                return "You have won the game!";
            }
        }

        foreach (var player in game.Players)
        {
            if (player.Key != context.ConnectionId)
                clients.Client(player.Key).SendAsync("SystemMessage", $"{userName} has played a card: {game.TopCard}").Wait();
            GameForSending gameForSeding = new GameForSending(game, player.Value);
            clients.Client(player.Key).SendAsync("SystemMessage", gameForSeding.ToConsoleString()).Wait();

        }

        return "Played " + game.TopCard.ToString();

    }
}