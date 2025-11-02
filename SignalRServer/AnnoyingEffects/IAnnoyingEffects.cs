using Microsoft.AspNetCore.SignalR;

namespace SignalRServer.AnnoyingEffects;


public interface IAnnoyingEffects
{
    Task Annoy(IClientProxy player, IClientProxy caller, string playerUsername = "", string callerUsername = "");
    Task AnnoyAll(Dictionary<string, IClientProxy> players, IClientProxy caller, string callerUsername = "");
}
