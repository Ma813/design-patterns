namespace SignalRServer.Models;

public class PlayerDeck
{
    public string Username { get; private set; }
    public List<BaseCard> Cards { get; private set; }

    public PlayerDeck(string username)
    {
        Cards = [];
        Username = username;

        var random = new Random();
        for (int i = 0; i < 7; i++)
        {
            BaseCard card = new NumberCard(((Colors)random.Next(0, 5)).ToString(), random.Next(0, 10));
            Cards.Add(card);
        }
    }

    public void AddCard(BaseCard card)
    {
        Cards.Add(card);
    }

    public bool RemoveCard(BaseCard card)
    {
        return Cards.Remove(card);
    }

    public int Count => Cards.Count;
}