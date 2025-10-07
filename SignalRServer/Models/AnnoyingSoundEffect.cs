using Microsoft.AspNetCore.SignalR;
using SignalRServer.Models;

public class AnnoyingSoundEffect : IAnnoyingEffects
{
    private SoundEffectAdaptee _adaptee;

    public AnnoyingSoundEffect()
    {
        _adaptee = new SoundEffectAdaptee();
    }

    public async Task AnnoyOne(IClientProxy player)
    {
        await _adaptee.SendSoundEffect(player);
    }
    public Task AnnoyAll(IClientProxy players)
    {
        Console.WriteLine("Annoying all players with sound effect is not allowed.");
        return Task.CompletedTask;
    }
}