using Microsoft.AspNetCore.SignalR;
using SignalRServer.AnnoyingEffects;
using SignalRServer.Hubs;
using SignalRServer.Models.CardPlacementStrategies;
using SignalRServer.Models.Chat;
using SignalRServer.Models.Game;
namespace SignalRServer.Models
{


    public interface IGameBuilder
{
    IGameBuilder SetRoomName(string roomName);
    IGameBuilder SetUserName(string userName);
    IGameBuilder SetContext(HubCallerContext context);
    IGameBuilder SetClients(IHubCallerClients clients);
    IGameBuilder SetGroups(IGroupManager groups);
    Task<AbstractGame> GetResultAsync();
}


}