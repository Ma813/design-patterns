using Microsoft.AspNetCore.SignalR;

namespace SignalRServer.Models
{
    public interface IAnnoyingEffects
    {
        Task Annoy(IClientProxy player, IClientProxy caller);
    }
}
