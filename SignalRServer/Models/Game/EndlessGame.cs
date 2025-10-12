namespace SignalRServer.Models;

public class EndlessGame : AbstractGame
{
    public override void End()
    {
        logger.LogInfo("Endless game does not end.");
        // Endless game does not end
    }
}