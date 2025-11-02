using SignalRServer.Models.Game;
using Microsoft.AspNetCore.SignalR;
using SignalRServer.AnnoyingEffects;
using SignalRServer.Hubs;
using SignalRServer.Models.CardPlacementStrategies;
using SignalRServer.Models.Chat;
namespace SignalRServer.Models
{

    public class ClassicUnoGameBuilder : IGameBuilder
{
    private readonly Facade _facade;
    private string _roomName;
    private string _userName;
    private HubCallerContext _context;
    private IHubCallerClients _clients;
    private IGroupManager _groups;

    public ClassicUnoGameBuilder(Facade facade)
    {
        _facade = facade;
    }

    public IGameBuilder SetRoomName(string roomName) { _roomName = roomName; return this; }
    public IGameBuilder SetUserName(string userName) { _userName = userName; return this; }
    public IGameBuilder SetContext(HubCallerContext context) { _context = context; return this; }
    public IGameBuilder SetClients(IHubCallerClients clients) { _clients = clients; return this; }
    public IGameBuilder SetGroups(IGroupManager groups) { _groups = groups; return this; }

    public async Task<AbstractGame> GetResultAsync()
    {
        await _facade.JoinRoom(
            roomName: _roomName,
            userName: _userName,
            Clients: _clients,
            Context: _context,
            Groups: _groups,
            botAmount: 0,
            gameMode: "Classic",
            cardPlacementStrategy: "UnoPlacementStrategy"
        );

        return _facade.GetGame(_roomName);
    }
}

}