namespace SignalRServer.Models
{
    public class Game
    {
        public List<PlayerDeck> PlayerDecks { get; set; }
        public Dictionary<string, string> Players { get; set; }
        public UnoCard topCard { get; set; }
        public bool isStarted { get; set; }

        public Game()
        {
            PlayerDecks = new List<PlayerDeck>();
            Players = new Dictionary<string, string>();
            topCard = UnoCard.GenerateCard();
            isStarted = false;
        }

        public void Start()
        {
            isStarted = true;
            foreach (var player in Players.Values)
            {
                PlayerDeck deck = new PlayerDeck(player);
                PlayerDecks.Add(deck);
            }

        }

    }
}