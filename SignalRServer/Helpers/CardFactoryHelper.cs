using SignalRServer.Factories;

namespace SignalRServer.Helpers
{
    public enum GameMode
    {
        SameColor,
        SameNumber,
        NumbersOnly,
        ActionOnly
    }

    public static class CardFactoryHelper
    {
        public static IUnoCardFactory GetFactory(GameMode mode, string? color = null, int? digit = null)
        {
            return mode switch
            {
                GameMode.SameColor => new SameColorFactory(color ?? "red"),
                GameMode.SameNumber => new SameNumberFactory(digit ?? 0),
                GameMode.NumbersOnly => new NumberOnlyFactory(),
                GameMode.ActionOnly => new ActionCardFactory(),
                _ => throw new ArgumentException("Invalid game mode")
            };
        }
    }
}