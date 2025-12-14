using SignalRServer.Models;
using  SignalRServer.Card;

public sealed class SuperCardGenerator : CardGenerator
{
    public SuperCardGenerator(IList<string> possibleColors)
        : base(possibleColors)
    {
    }

    // Override chances so that GenerateRandomCard() includes super cards
    protected override int NumberChance => 60;
    protected override int PowerChance  => 20;
    protected override int SuperChance  => 20;

    protected override UnoCard GenerateNumberCard()
    {
        string color = PossibleColors[RNG.Next(PossibleColors.Count)];
        int digit = RNG.Next(10);
        return new NumberCard(color, digit);
    }

    protected override UnoCard GeneratePowerCard()
    {
        string color = PossibleColors[RNG.Next(PossibleColors.Count)];
        string[] power = { "Skip", "Draw" };
        string powerType = power[RNG.Next(power.Length)];

        return new PowerCard(color, powerType);
    }

    protected override UnoCard GenerateSuperCard()
    {
        string color = PossibleColors[RNG.Next(PossibleColors.Count)];
        string[] power = { "Skip", "Draw" };
        string powerType = power[RNG.Next(power.Length)];

        UnoCard baseCard = new PowerCard(color, powerType);

        for (int i = 0; i < 3; i++)
        {
            if (RNG.Next(2) == 0)
                baseCard = new SkipTurnDecorator(baseCard);
            else
                baseCard = new DrawCardDecorator(baseCard);
        }

        return baseCard;
    }
}
