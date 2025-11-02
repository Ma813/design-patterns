using SignalRServer.Models.Game;
using Microsoft.AspNetCore.SignalR;
using SignalRServer.AnnoyingEffects;
using SignalRServer.Hubs;
using SignalRServer.Models.CardPlacementStrategies;
using SignalRServer.Models.Chat;
namespace SignalRServer.Models
{
    public class GameDirector
{
    private IGameBuilder _builder;

    public GameDirector(IGameBuilder builder)
    {
        _builder = builder;
    }

    public async Task<AbstractGame> ConstructAsync(string roomName, string userName, IHubCallerClients clients, HubCallerContext context, IGroupManager groups)
    {
        return await _builder
            .SetRoomName(roomName)
            .SetUserName(userName)
            .SetClients(clients)
            .SetContext(context)
            .SetGroups(groups)
            .GetResultAsync();
    }
}

}