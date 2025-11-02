using SignalRServer.Models;

namespace SignalRServer.Helpers;

public enum GameType
{
    Classic,
    Endless,
    DrawToMatch
}

public abstract class AbstractGameCreator
{
    public abstract AbstractGame CreateGame(
        GameType gameMode = GameType.Classic,
        CardGeneratingMode cardGeneratingMode = CardGeneratingMode.Normal,
        StrategyType cardPlacementStrategy = StrategyType.Normal,
        string roomName = "DefaultRoom"
    );
}