using Microsoft.AspNetCore.SignalR;
using SignalRServer.Models;

namespace SignalRServer.AnnoyingEffects;

public class SoundEffectProxy : ISoundEffect
{
    private static readonly Logger logger = Logger.GetInstance();
    private readonly ISoundEffect realSoundEffect;

    // key: muted player, value: list of players who muted him
    private readonly Dictionary<string, List<string>> mutedPlayers = [];

    public SoundEffectProxy(ISoundEffect realSoundEffect)
    {
        this.realSoundEffect = realSoundEffect;
    }

    public async Task SendSoundEffect(
        IClientProxy player,
        IClientProxy caller,
        string playerUsername,
        string callerUsername)
    {
        if (mutedPlayers.ContainsKey(callerUsername) &&
            mutedPlayers[callerUsername].Contains(playerUsername))
        {
            logger.LogInfo(
                $"{callerUsername} tried to annoy muted user {playerUsername}, but they are muted.");
            return;
        }
        await realSoundEffect.SendSoundEffect(
            player, caller, playerUsername, callerUsername);
    }

    public async Task ToggleMutePlayer(string mutedPlayer, string mutingPlayer)
    {
        if (!mutedPlayers.ContainsKey(mutedPlayer))
        {
            mutedPlayers[mutedPlayer] = [];
        }

        if (mutedPlayers[mutedPlayer].Contains(mutingPlayer))
        {
            logger.LogInfo($"{mutingPlayer} has unmuted {mutedPlayer}.");
            mutedPlayers[mutedPlayer].Remove(mutingPlayer);
        }
        else
        {
            logger.LogInfo($"{mutingPlayer} has muted {mutedPlayer}.");
            mutedPlayers[mutedPlayer].Add(mutingPlayer);
        }

        logger.LogInfo($"Current muted players: {System.Text.Json.JsonSerializer.Serialize(mutedPlayers)}");
        await Task.CompletedTask;
    }
}
