using  SignalRServer.Card;

namespace SignalRServer.Models
{

public sealed class StandardCardGenerator : CardGenerator
{
    public StandardCardGenerator(IList<string> possibleColors)
        : base(possibleColors)
    {
    }

    protected override UnoCard GenerateNumberCard()
    {
        string color = PossibleColors[RNG.Next(PossibleColors.Count)];
        int digit = RNG.Next(10);
        return UnoCardFlyweightFactory.GetNumberCard(color, digit);
    }

    protected override UnoCard GeneratePowerCard()
    {
        string color = PossibleColors[RNG.Next(PossibleColors.Count)];
        string[] power = { "Skip", "Draw" };
        string powerType = power[RNG.Next(power.Length)];

        return UnoCardFlyweightFactory.GetPowerCard(color, powerType);
    }

    protected override UnoCard GenerateSuperCard()
    {
        throw new NotSupportedException("Standard hands do not include super cards.");
    }
}
}