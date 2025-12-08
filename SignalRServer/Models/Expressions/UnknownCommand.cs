using Microsoft.AspNetCore.SignalR;
using SignalRServer.Hubs;

namespace SignalRServer.Expressions;

public class UnknownExpression : IExpression
{
    public string Interpret(string command, HubCallerContext context, IHubCallerClients clients, IGroupManager groups)
    {
        return $"Command {command} not recognized. Type 'help' for a list of available commands.";
    }
}