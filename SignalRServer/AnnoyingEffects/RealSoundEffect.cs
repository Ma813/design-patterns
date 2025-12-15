using Microsoft.AspNetCore.SignalR;
using SignalRServer.Models;

namespace SignalRServer.AnnoyingEffects;

public class RealSoundEffect : ISoundEffect
{
    private static readonly Logger logger = Logger.GetInstance();

    public async Task SendSoundEffect(
        IClientProxy player,
        IClientProxy caller,
        string playerUsername,
        string callerUsername)
    {
        logger.LogInfo($"{callerUsername} is annoying user {playerUsername} with sound effect!");
        await player.SendAsync("PlaySound");
    }
}
