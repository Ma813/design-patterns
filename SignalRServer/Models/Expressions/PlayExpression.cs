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

        if (game.PlayerDecks.Where(pd => pd.Username == game.Players[context.ConnectionId]).First().Count <= cardIndex || cardIndex < 0)
            return "Error: Invalid card index.";

        string userName = game.Players[context.ConnectionId];


        string result = facade.PlayCard(game.RoomName, userName, cardIndex, clients, context.ConnectionId).Result;
        if (result != "OK" && result != "WIN")
            return result;

        string answer = "Played " + game.TopCard.ToString();

        if (result == "WIN")
        {
            answer += $"\nCongratulations {userName}, you have won the game!";
        }

        return answer;

    }
}