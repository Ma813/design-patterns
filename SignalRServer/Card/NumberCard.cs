using SignalRServer.Models.Game;

namespace SignalRServer.Card;

public class NumberCard : UnoCard
{
    public NumberCard(string color, int digit)
        : base(color, new NumberCardImplementation(color, digit))
    {
        Digit = digit;
    }

    public override void Play(AbstractGame game)
    {
        implementation.ExecuteEffect(game);
    }
}