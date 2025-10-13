namespace SignalRServer.Helpers;

public enum StrategyType
{
    Normal,
    ColorOnly,
    FaceOnly,
    AdjacentNumber
}

public static class StrategyCreator
{
    public static ICardPlacementStrategy GetStrategy(StrategyType type)
    {
        return type switch
        {
            StrategyType.Normal => new CardPlacementStrategy(),
            StrategyType.ColorOnly => new ColorOnlyPlacementStrategy(),
            StrategyType.FaceOnly => new FaceOnlyPlacementStrategy(),
            StrategyType.AdjacentNumber => new AdjacentNumberPlacementStrategy(),
            _ => throw new ArgumentException("Invalid strategy type")
        };
    }
}