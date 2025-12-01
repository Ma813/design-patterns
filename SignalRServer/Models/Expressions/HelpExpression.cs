using Microsoft.AspNetCore.SignalR;
using SignalRServer.Hubs;

namespace SignalRServer.Expressions;

public class HelpExpression : IExpression
{
    public string Interpret(string command, HubCallerContext context, IHubCallerClients clients, IGroupManager groups)
    {
        return "Available commands:\n" +
               " help - Show this help message\n" +
               " join <RoomName> <UserName> - Join a room\n" +
               " start - Start the game in the current room\n" +
               " play <cardIndex> - Play a card from your hand\n" +
               " draw - Draw a card from the deck\n";

    }
}