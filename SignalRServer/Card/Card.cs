using SignalRServer.Models.CardPlacementStrategies;
using SignalRServer.Models.Game;

namespace SignalRServer.Card;

public abstract partial class UnoCard : ICardComponent
{
    private static readonly Random _rng = new Random();
    public static readonly List<string> PossibleColors = new List<string> { "red", "green", "blue", "yellow" };
    public string Color { get; set; }
    public int Digit { get; set; }
    public virtual string Name => implementation.GetEffectDescription();
    public ICardImplementation implementation;

    protected UnoCard(string color, ICardImplementation implementation)
    {
        Color = color;
        this.implementation = implementation;
    }

    public abstract void Play(AbstractGame game);

    public virtual bool CanPlayOn(UnoCard topCard, ICardPlacementStrategy cardPlacementStrategy)
    {
        return cardPlacementStrategy.CanPlaceCard(topCard, this);
    }

    // Default implementation for leaf nodes (throws exceptions as they don't support children)
    public virtual void Add(ICardComponent component)
    {
        throw new NotSupportedException("Cannot add components to a leaf card.");
    }

    public virtual void Remove(ICardComponent component)
    {
        throw new NotSupportedException("Cannot remove components from a leaf card.");
    }

    public virtual ICardComponent GetChild(int index)
    {
        throw new NotSupportedException("Leaf cards don't have children.");
    }

    public virtual int GetChildCount()
    {
        return 0;
    }

    public override string ToString()
    {
        return $"{Color} {Name}";
    }
}

public partial class UnoCard
{
    public static UnoCard GenerateCard(bool useFlyweight = true)
    {
        string color = PossibleColors[_rng.Next(PossibleColors.Count)];

        if (_rng.Next(100) < 70) // 70% chance for number card
        {
            int digit = _rng.Next(0, 10);
            if (useFlyweight)
            {
                return UnoCardFlyweightFactory.GetNumberCard(color, digit);
            }

            return new NumberCard(color, digit);
        }
        else
        {
            string[] powerTypes = { "Skip", "Draw", "NewRule" };
            string powerType = powerTypes[_rng.Next(powerTypes.Length)];
            if (useFlyweight)
            {
                return UnoCardFlyweightFactory.GetPowerCard(color, powerType);
            }
            return new PowerCard(color, powerType);
        }
    }

    public static UnoCard GenerateSuperCard()
    {
        var random = new Random();
        string color = PossibleColors[random.Next(PossibleColors.Count)];

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