using SignalRServer.Helpers;

namespace SignalRServer.Models;

public class EndlessGame : AbstractGame
{
    public EndlessGame(CardGeneratingMode cardGeneratingMode) : base(cardGeneratingMode)
    {
    }

    public override void End()
    {
        logger.LogInfo("Endless game does not end.");
        // Endless game does not end
    }
}