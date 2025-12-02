using Microsoft.AspNetCore.SignalR;
using SignalRServer.Hubs;

namespace SignalRServer.Expressions;

public interface IExpression
{
    string Interpret(string command, HubCallerContext context, IHubCallerClients clients, IGroupManager groups);
}