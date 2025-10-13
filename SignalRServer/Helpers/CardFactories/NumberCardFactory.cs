using SignalRServer.Models;

namespace SignalRServer.Helpers;

public class NumberCardFactory : ICardFactory
{
    private readonly Random random = new();

    public BaseCard GenerateCard()
    {
        Colors color = (Colors)random.Next(0, Enum.GetValues(typeof(Colors)).Length);
        int digit = random.Next(0, 10);
        return new NumberCard(color.ToString(), digit);
    }
}
