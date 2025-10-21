using Microsoft.AspNetCore.SignalR;

public class SoundEffectAdaptee
{
    // Key - player who is muted
    // Value - list of players who muted the key player
    private Dictionary<string, List<string>> mutedPlayers = new Dictionary<string, List<string>>();


    public async Task SendSoundEffect(IClientProxy player, IClientProxy caller, string playerUsername, string callerUsername)
    {
        if (mutedPlayers.ContainsKey(playerUsername) && mutedPlayers[playerUsername].Contains(callerUsername))
        {
            // Player is muted, do not send sound effect
            System.Console.WriteLine($"{caller} tried to annoy muted user {player} with sound effect, but they are muted.");
            return;
        }

        await player.SendAsync("PlaySound", "annoying.mp3");
        // Placeholder implementation
        System.Console.WriteLine($"{caller} is annoying user {player} with sound effect!");
    }

    public async Task ToggleMutePlayer(string mutedPlayer, string mutingPlayer)
    {
        System.Console.WriteLine($"Current muted players: {System.Text.Json.JsonSerializer.Serialize(mutedPlayers)}");

        if (!mutedPlayers.ContainsKey(mutedPlayer))
        {
            mutedPlayers[mutedPlayer] = new List<string>();
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