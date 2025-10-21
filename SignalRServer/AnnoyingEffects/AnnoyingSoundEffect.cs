using Microsoft.AspNetCore.SignalR;
using SignalRServer.Models;

public class AnnoyingSoundEffect : IAnnoyingEffects
{
    private static SoundEffectAdaptee? _adaptee;

    public AnnoyingSoundEffect(SoundEffectAdaptee adaptee)
    {
        _adaptee = adaptee;
    }
    public async Task Annoy(IClientProxy player, IClientProxy caller, string playerUsername = "", string callerUsername = "")
    {
        await _adaptee.SendSoundEffect(player, caller, playerUsername, callerUsername);
    }
}