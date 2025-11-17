using Microsoft.AspNetCore.SignalR;

namespace SignalRServer.AnnoyingEffects;

public class AnnoyingSoundEffect : IAnnoyingEffects
{
    private static SoundEffectAdaptee? _adaptee;

    public AnnoyingSoundEffect(SoundEffectAdaptee adaptee)
    {
        _adaptee = adaptee;
    }

    public async Task Annoy(IClientProxy player, IClientProxy caller, string playerUsername = "", string callerUsername = "")
    {
        if (_adaptee == null)
        {
            throw new InvalidOperationException("SoundEffectAdaptee is not initialized.");
        }
        await _adaptee.SendSoundEffect(player, caller, playerUsername, callerUsername);
    }

    public async Task AnnoyAll(Dictionary<string, IClientProxy> players, IClientProxy caller, string callerUsername = "")
    {
        // Since sound effects are individual, we can't annoy all at once.
        if (players == null || players.Count == 0)
        {
            throw new ArgumentException("No players to annoy.", nameof(players));
        }

        foreach (var player in players)
        {
            await Annoy(player.Value, caller, player.Key, callerUsername);
        }
    }
}