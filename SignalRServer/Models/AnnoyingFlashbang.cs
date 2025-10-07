using Microsoft.AspNetCore.SignalR;

namespace SignalRServer.Models
{
    public class AnnoyingFlashbang : IAnnoyingEffects
    {
        public async Task AnnoyAll(IClientProxy players)
        {
            await players.SendAsync("Flashbang");
            System.Console.WriteLine($"Annoying everyone with flashbang: {players}");
        }

        public async Task AnnoyOne(IClientProxy player)
        {
            await player.SendAsync("Flashbang");
            System.Console.WriteLine($"Annoying user {player} with flashbang!");
        }
    }
}
