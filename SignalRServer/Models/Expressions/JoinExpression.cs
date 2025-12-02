using System.Text.RegularExpressions;
using Microsoft.AspNetCore.SignalR;
using SignalRServer.Hubs;
using SignalRServer.Models;
using SignalRServer.Models.Game;

namespace SignalRServer.Expressions;

public class JoinExpression : IExpression
{
    public string Interpret(string command, HubCallerContext context, IHubCallerClients clients, IGroupManager groups)
    {
        // Command format: "join RoomName"
        string[] parts = command.Split(' ', 3, StringSplitOptions.RemoveEmptyEntries);

        if (parts.Length < 3)
            return "Error: Room name and User name are required. Usage: join <RoomName> <UserName>";

        string roomName = parts[1]; // Extract RoomName from command
        string userName = parts[2]; // Extract UserName from command

        string connectionId = context.ConnectionId;

        // Call your Facade method
        Facade facade = Facade.GetInstance(clients as IHubContext<PlayerHub>);
        facade.JoinRoom(
            roomName,
            userName,
            clients,
            context,
            groups,
            0,
            "Classic",
            "UnoPlacementStrategy",
            "Classic"
        ).Wait();

        AbstractGame game = facade.GetGameByConnection(context);
        if (game == null)
            return "Error: Could not join the room.";

        string playerList = "Players in the room:\n";

        foreach (var player in game.Players)
        {
            playerList += $"- {player.Value}\n";

        }
        
        return $"Joined room {roomName}.\n{playerList}";
    }
}