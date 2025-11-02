using SignalRServer.Models.Game;

namespace SignalRServer.Card;

public class PowerCardImplementation : ICardImplementation
{
    public string Color { get; }
    public string PowerType { get; }

    public PowerCardImplementation(string color, string powerType)
    {
        Color = color;
        PowerType = powerType;
    }

    public void ExecuteEffect(AbstractGame game)
    {
        Console.WriteLine($"Executing Power Card Effect: {PowerType}");
        switch (PowerType)
        {
            case "Skip":
                game.NextPlayer(Models.Action.place);
                break;
            case "Draw":
                game.NextDrawCard();
                break;
        }
    }

    public string GetEffectDescription()
    {
        return PowerType;
    }
}