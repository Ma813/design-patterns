using Microsoft.AspNetCore.SignalR;

public class SoundEffectAdaptee
{
    // Key - player who is muted
    // Value - list of players who muted the key player
    private Dictionary<IClientProxy, List<IClientProxy>> mutedPlayers = new Dictionary<IClientProxy, List<IClientProxy>>();


    public async Task SendSoundEffect(IClientProxy player, IClientProxy caller)
    {
        if (mutedPlayers.ContainsKey(player) && mutedPlayers[player] == caller)
        {
            // Player is muted, do not send sound effect
            System.Console.WriteLine($"{caller} tried to annoy muted user {player} with sound effect, but they are muted.");
            return;
        }

        await player.SendAsync("PlaySound", "annoying.mp3");
        // Placeholder implementation
        System.Console.WriteLine($"{caller} is annoying user {player} with sound effect!");
    }

    public async Task ToggleMutePlayer(IClientProxy mutedPlayer, IClientProxy mutingPlayer)
    {
        if (!mutedPlayers.ContainsKey(mutedPlayer))
        {
            mutedPlayers[mutedPlayer] = new List<IClientProxy>();
        }

        if (mutedPlayers[mutedPlayer].Contains(mutingPlayer))
        {
            mutedPlayers[mutedPlayer].Remove(mutingPlayer);
            System.Console.WriteLine($"{mutingPlayer} has unmuted {mutedPlayer}.");
        }
        else
        {
            mutedPlayers[mutedPlayer].Add(mutingPlayer);
            System.Console.WriteLine($"{mutingPlayer} has muted {mutedPlayer}.");
        }

        await Task.CompletedTask;
    }
}