using SignalRServer.Helpers;

namespace SignalRServer.Models;

public class EndlessGame : AbstractGame
{
    public EndlessGame(CardGeneratingMode cardGeneratingMode, StrategyType placementStrategy, string roomName) : base(cardGeneratingMode, placementStrategy, roomName)
    {
    }

    public override void End()
    {
        logger.LogInfo("Endless game does not end.");
        // Endless game does not end
    }
}