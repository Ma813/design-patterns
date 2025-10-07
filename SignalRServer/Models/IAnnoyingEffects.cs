using Microsoft.AspNetCore.SignalR;

namespace SignalRServer.Models
{
    public interface IAnnoyingEffects
    {
        Task AnnoyAll(IClientProxy players);
        Task AnnoyOne(IClientProxy player);
    }
}
