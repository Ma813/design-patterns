using SignalRServer.Models.CardPlacementStrategies;
using SignalRServer.Models.Game;

namespace SignalRServer.Card;

public abstract partial class UnoCard
{
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

    public override string ToString()
    {
        return $"{Color} {Name}";
    }
}

public partial class UnoCard
{
    public static UnoCard GenerateCard()
    {
        var random = new Random();
        string color = PossibleColors[random.Next(PossibleColors.Count)];
        
        if (random.Next(100) < 70)
        {
            int digit = random.Next(0, 10);
            return new NumberCard(color, digit);
        }
        else
        {
            string[] powerTypes = { "Skip", "Draw", "NewRule" };
            string powerType = powerTypes[random.Next(powerTypes.Length)];
            return new PowerCard(color, powerType);
        }
    }

    public static UnoCard GenerateSuperCard()
    {
        var random = new Random();
        
        // Start with a base card (using Bridge pattern)
        string color = PossibleColors[random.Next(PossibleColors.Count)];
        string[] powerTypes = { "Skip", "Draw" };
        string powerType = powerTypes[random.Next(powerTypes.Length)];
        UnoCard baseCard = new PowerCard(color, powerType);
        
        // Apply 3 layers of decorators (Decorator pattern)
        for (int i = 0; i < 3; i++)
        {
            if (random.Next(2) == 0)
            {
                baseCard = new SkipTurnDecorator(baseCard);
            }
            else
            {
                baseCard = new DrawCardDecorator(baseCard);
            }
        }
        
        return baseCard;
    }
}