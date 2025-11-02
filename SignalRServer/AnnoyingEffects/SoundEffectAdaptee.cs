using Microsoft.AspNetCore.SignalR;
using SignalRServer.Models;

namespace SignalRServer.AnnoyingEffects;

public class SoundEffectAdaptee
{
    private static readonly Logger logger = Logger.GetInstance();

    private readonly Dictionary<string, List<string>> mutedPlayers = []; // key: muted player, value: list of players who muted him

    public async Task SendSoundEffect(IClientProxy player, IClientProxy caller, string playerUsername, string callerUsername)
    {
        if (mutedPlayers.ContainsKey(callerUsername) && mutedPlayers[callerUsername].Contains(playerUsername))
        {
            // Player is muted, do not send sound effect
            logger.LogInfo($"{callerUsername} tried to annoy muted user {playerUsername} with sound effect, but they are muted.");
            return;
        }

        logger.LogInfo($"{callerUsername} is annoying user {playerUsername} with sound effect!");
        await player.SendAsync("PlaySound", "annoying.mp3");
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