using Microsoft.AspNetCore.SignalR;

namespace SignalRServer.AnnoyingEffects;

public interface ISoundEffect
{
    Task SendSoundEffect(
        IClientProxy player,
        IClientProxy caller,
        string playerUsername,
        string callerUsername);
}
