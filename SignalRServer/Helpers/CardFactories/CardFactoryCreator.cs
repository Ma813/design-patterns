namespace SignalRServer.Helpers;

public enum CardGeneratingMode
{
    Normal,
    SameColor,
    NumbersOnly,
    ActionOnly
}

public static class CardFactoryCreator
{
    public static ICardFactory GetFactory(CardGeneratingMode mode, string? color = null, int? digit = null)
    {
        return mode switch
        {
            CardGeneratingMode.Normal => new CardFactory(),
            CardGeneratingMode.SameColor => new SameColorFactory(color ?? "red"),
            CardGeneratingMode.NumbersOnly => new NumberCardFactory(),
            CardGeneratingMode.ActionOnly => new ActionCardFactory(),
            _ => throw new ArgumentException("Invalid game mode")
        };
    }
}