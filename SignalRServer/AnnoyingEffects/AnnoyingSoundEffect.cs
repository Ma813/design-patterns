using Microsoft.AspNetCore.SignalR;
using SignalRServer.Models;

public class AnnoyingSoundEffect : IAnnoyingEffects
{
    private SoundEffectAdaptee _adaptee;

    public AnnoyingSoundEffect()
    {
        _adaptee = new SoundEffectAdaptee();
    }

    public async Task Annoy(IClientProxy player, IClientProxy caller)
    {
        await _adaptee.SendSoundEffect(player, caller);
    }
}