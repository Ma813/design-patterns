namespace SignalRServer.Models.Game;

// Abstract factory
public class GameCreator : AbstractGameCreator
{
    private static readonly Logger logger = Logger.GetInstance();

    public override AbstractGame CreateGame(string gameMode = "Classic", string roomName = "DefaultRoom")
    {
        switch (gameMode)
        {
            case "Classic":
                logger.LogInfo("Classic mode selected");
                return new Game(roomName);
            // Future game modes can be added here
            case "Endless":
                logger.LogInfo("Endless mode selected");
                return new EndlessGame(roomName);
            case "DrawToMatch":
                logger.LogInfo("DrawToMatch mode selected");
                return new DrawToMatchGame(roomName);
            default:
                throw new ArgumentException("Invalid game mode");
        }
    }
}
