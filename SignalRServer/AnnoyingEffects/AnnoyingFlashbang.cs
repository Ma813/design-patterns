using Microsoft.AspNetCore.SignalR;

namespace SignalRServer.Models
{
    public class AnnoyingFlashbang : IAnnoyingEffects
    {
        public async Task Annoy(IClientProxy player, IClientProxy caller)
        {
            await player.SendAsync("Flashbang");
            System.Console.WriteLine($"{caller} is annoying user {player} with flashbang!");
        }
    }
}
