using SignalRServer.Models;

namespace SignalRServer.Helpers;

public class GameCreator : AbstractGameCreator
{
    private static readonly Logger logger = Logger.GetInstance();

    public override AbstractGame CreateGame(
        string gameMode = "Classic",
        CardGeneratingMode cardGeneratingMode = CardGeneratingMode.Normal,
        StrategyType cardPlacementStrategy = StrategyType.Normal)
    {
        switch (gameMode)
        {
            case "Classic":
                logger.LogInfo("Classic mode selected");
                return new Game(cardGeneratingMode, cardPlacementStrategy);

            // Future game modes can be added here
            case "Endless":
                logger.LogInfo("Endless mode selected");
                return new EndlessGame(cardGeneratingMode, cardPlacementStrategy);
            case "DrawToMatch":
                logger.LogInfo("DrawToMatch mode selected");
                return new DrawToMatchGame(cardGeneratingMode, cardPlacementStrategy);
            default:
                throw new ArgumentException("Invalid game mode");
        }
    }
}
