using SignalRServer.Factories;

namespace SignalRServer.Models
{
    public class PlayerDeck
    {
        public List<UnoCard> Cards { get; private set; }
        public string Username { get; private set; }
        public PlayerDeck(string username, IUnoCardFactory cardFactory)
        {
            Cards = new List<UnoCard>();
            Username = username; for (int i = 0; i < 7; i++)
            {
                UnoCard card = cardFactory.GenerateCard();
                Cards.Add(card);
            }
        }

        public void AddCard(UnoCard card)
        {
            Cards.Add(card);
        }

        public bool RemoveCard(UnoCard card)
        {
            return Cards.Remove(card);
        }

        public int Count => Cards.Count;
    }
}