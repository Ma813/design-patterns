using Microsoft.AspNetCore.SignalR;
using SignalRServer.Models;

namespace SignalRServer.AnnoyingEffects;

public class AnnoyingFlashbang : IAnnoyingEffects
{
    private static readonly Logger logger = Logger.GetInstance();

    public async Task Annoy(IClientProxy player, IClientProxy caller, string playerUsername = "", string callerUsername = "")
    {
        logger.LogInfo($"{callerUsername} is annoying user {playerUsername} with flashbang!");
        await player.SendAsync("Flashbang");
    }

    public async Task AnnoyAll(Dictionary<string, IClientProxy> players, IClientProxy caller, string callerUsername = "")
    {
        foreach (var player in players)
        {
            logger.LogInfo($"{callerUsername} is annoying user {player.Key} with flashbang!");
            await player.Value.SendAsync("Flashbang");
        }
    }
}