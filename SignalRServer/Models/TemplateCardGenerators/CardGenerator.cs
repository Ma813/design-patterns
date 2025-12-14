
using  SignalRServer.Card;

namespace SignalRServer.Models
{
public abstract class CardGenerator
{
    protected readonly IList<string> PossibleColors;
    protected readonly Random RNG = new Random();

    protected CardGenerator(IList<string> possibleColors)
    {
        PossibleColors = possibleColors;
    }

    // TEMPLATE METHOD: generates a player's starting hand
    public List<UnoCard> GenerateStartingHand(int count)
    {
        var hand = new List<UnoCard>();

        for (int i = 0; i < count; i++)
            hand.Add(GenerateRandomCard());

        return hand;
    }

    // --- Template hook properties (override in subclasses) ---
    protected virtual int NumberChance => 70;   // %
    protected virtual int PowerChance => 30;    // %
    protected virtual int SuperChance => 0;     // %

    // --- Methods subclasses MUST implement ---
    protected abstract UnoCard GenerateNumberCard();
    protected abstract UnoCard GeneratePowerCard();
    protected abstract UnoCard GenerateSuperCard(); // May throw if unsupported

    // -----------------------------------------
    // REQUIRED: GenerateRandomCard()
    // -----------------------------------------
    public virtual UnoCard GenerateRandomCard()
    {
        int roll = RNG.Next(100);

        if (roll < SuperChance)
            return GenerateSuperCard();

        roll -= SuperChance;
        if (roll < NumberChance)
            return GenerateNumberCard();

        return GeneratePowerCard();
    }
}

}