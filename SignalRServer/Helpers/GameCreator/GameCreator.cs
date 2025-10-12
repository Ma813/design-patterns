using SignalRServer.Models;

namespace SignalRServer.Helpers;

public class GameCreator : AbstractGameCreator
{
    private static readonly Logger logger = Logger.GetInstance();

    public override AbstractGame CreateGame(string gameMode = "Classic", CardGeneratingMode cardGeneratingMode = CardGeneratingMode.Normal)
    {
        switch (gameMode)
        {
            case "Classic":
                logger.LogInfo("Classic mode selected");
                return new Game(cardGeneratingMode);

            // Future game modes can be added here
            case "Endless":
                logger.LogInfo("Endless mode selected");
                return new EndlessGame(cardGeneratingMode);
            case "DrawToMatch":
                logger.LogInfo("DrawToMatch mode selected");
                return new DrawToMatchGame(cardGeneratingMode);
            default:
                throw new ArgumentException("Invalid game mode");
        }
    }
}
