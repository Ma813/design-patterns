using SignalRServer.Models;

namespace SignalRServer.Helpers;

public class GameCreator : AbstractGameCreator
{
    private static readonly Logger logger = Logger.GetInstance();

    public override AbstractGame CreateGame(
        GameType gameMode = GameType.Classic,
        CardGeneratingMode cardGeneratingMode = CardGeneratingMode.Normal,
        StrategyType cardPlacementStrategy = StrategyType.Normal,
        string roomName = "DefaultRoom")
    {
        return gameMode switch
        {
            GameType.Classic => new Game(cardGeneratingMode, cardPlacementStrategy, roomName),
            GameType.Endless => new EndlessGame(cardGeneratingMode, cardPlacementStrategy, roomName),
            GameType.DrawToMatch => new DrawToMatchGame(cardGeneratingMode, cardPlacementStrategy, roomName),
            _ => throw new ArgumentException("Invalid Game Mode")
        };
    }
}
