using SignalRServer.Card;
using SignalRServer.Models.Game;

public class NumberCardImplementation : ICardImplementation
{
    public int Number { get; }
    public string Color { get; }

    public NumberCardImplementation(string color, int number)
    {
        Color = color;
        Number = number;
    }

    public void ExecuteEffect(AbstractGame game)
    {
        // Number cards typically have no special effect
    }

    public string GetEffectDescription()
    {
        return Number.ToString();
    }
}