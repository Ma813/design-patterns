using SignalRServer.Card;

namespace SignalRServer.Models.Memento;

public class PlayerHandMemento
{
    public List<UnoCard> CardsSnapshot { get; }

    public PlayerHandMemento(List<UnoCard> cards)
    {
        // deep copy of hand (new list, same card instances is fine)
        CardsSnapshot = new List<UnoCard>(cards);
    }
}
