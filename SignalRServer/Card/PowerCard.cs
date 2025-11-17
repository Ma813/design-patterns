using SignalRServer.Models.Game;

namespace SignalRServer.Card;

public class PowerCard : UnoCard
{
    public string PowerType { get; }

    public PowerCard(string color, string powerType)
        : base(color, new PowerCardImplementation(color, powerType))
    {
        PowerType = powerType;
        Digit = -1; // Power cards don't have digits
    }

    public override void Play(AbstractGame game)
    {
        implementation.ExecuteEffect(game);
    }
}