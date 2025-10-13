using SignalRServer.Models;

namespace SignalRServer.Helpers;

public class GameCreator : AbstractGameCreator
{
    private static readonly Logger logger = Logger.GetInstance();

    public override AbstractGame CreateGame(
        GameType gameMode = GameType.Classic,
        CardGeneratingMode cardGeneratingMode = CardGeneratingMode.Normal,
        StrategyType cardPlacementStrategy = StrategyType.Normal)
    {
        return gameMode switch
        {
            GameType.Classic => new Game(cardGeneratingMode, cardPlacementStrategy),
            GameType.Endless => new EndlessGame(cardGeneratingMode, cardPlacementStrategy),
            GameType.DrawToMatch => new DrawToMatchGame(cardGeneratingMode, cardPlacementStrategy),
            _ => throw new ArgumentException("Invalid Game Mode")
        };
    }
}
