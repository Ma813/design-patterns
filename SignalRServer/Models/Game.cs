namespace SignalRServer.Models
{
    public class Game : ISubject
    {
        public List<PlayerDeck> PlayerDecks { get; set; }
        public Dictionary<string, string> Players { get; set; } // {connectionId: username}
        public UnoCard topCard { get; set; }
        public bool isStarted { get; set; }
        public int currentPlayerIndex { get; set; }
        public int direction { get; set; } // 1 for clockwise, -1 for counter-clockwise
        // To swap direction, just multiply by -1

        public Dictionary<string, int> placedCardCount { get; set; }

        public Game()
        {
            PlayerDecks = new List<PlayerDeck>();
            Players = new Dictionary<string, string>();
            topCard = UnoCard.GenerateCard();
            isStarted = false;
            currentPlayerIndex = 0;
            direction = 1;
            placedCardCount = new Dictionary<string, int>();

            AttachObservers();
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

        public void NextPlayer()
        {
            currentPlayerIndex = (currentPlayerIndex + direction + PlayerDecks.Count) % PlayerDecks.Count;
        }

        public void AttachObservers()
        {
            base.Add(new CardCountUpdater());
        }

        public void SetCardCount(Dictionary<string, int> cardCount)
        {
            this.placedCardCount = cardCount;
        }

    }
}