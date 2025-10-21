using Microsoft.AspNetCore.SignalR;

namespace SignalRServer.Models
{
    public class AnnoyingFlashbang : IAnnoyingEffects
    {
        public async Task Annoy(IClientProxy player, IClientProxy caller, string playerUsername = "", string callerUsername = "")
        {
            await player.SendAsync("Flashbang");
            System.Console.WriteLine($"{callerUsername} is annoying user {playerUsername} with flashbang!");
        }

        public async Task AnnoyAll(Dictionary<string, IClientProxy> players, IClientProxy caller, string callerUsername = "")
        {
            foreach (var player in players)
            {
                await player.Value.SendAsync("Flashbang");
                System.Console.WriteLine($"{callerUsername} is annoying user {player.Key} with flashbang!");
            }
        }
    }
}
