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
        string[] power = { "Skip", "Draw","RestoreHand" };
        string powerType = power[RNG.Next(power.Length)];

        return new PowerCard(color, powerType);
    }

    protected override UnoCard GenerateSuperCard()
    {
        var random = new Random();
        string color = PossibleColors[RNG.Next(PossibleColors.Count)];

        // Create a composite card using Composite pattern
        var compositeCard = new CompositeCard(color);

        // Add base power card
        string[] powerTypes = { "Skip", "Draw" };
        string powerType = powerTypes[random.Next(powerTypes.Length)];
        compositeCard.Add(new PowerCard(color, powerType));

        // Add additional effects (3 random effects)
        for (int i = 0; i < 3; i++)
        {
            if (random.Next(2) == 0)
            {
                compositeCard.Add(new SkipEffectCard(color));
            }
            else
            {
                compositeCard.Add(new DrawEffectCard(color));
            }
        }

        return compositeCard;
    }
}
