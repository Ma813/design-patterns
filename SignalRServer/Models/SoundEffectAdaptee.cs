using Microsoft.AspNetCore.SignalR;

public class SoundEffectAdaptee
{
    public async Task SendSoundEffect(IClientProxy player)
    {
        await player.SendAsync("PlaySound", "annoying.mp3");
        // Placeholder implementation
        System.Console.WriteLine($"Annoying user {player} with sound effect!");
    }
}