using SignalRServer.Factories;
using SignalRServer.Helpers;

namespace SignalRServer.Models
{
    public class Game
    {
        public List<PlayerDeck> PlayerDecks { get; set; }
        public Dictionary<string, string> Players { get; set; } // {connectionId: username}
        public UnoCard topCard { get; set; }
        public bool isStarted { get; set; }
        public int currentPlayerIndex { get; set; }
        public int direction { get; set; } // 1 for clockwise, -1 for counter-clockwise
        private IUnoCardFactory cardFactory;
        // To swap direction, just multiply by -1

        public Game(GameMode factoryType)
        {
            PlayerDecks = new List<PlayerDeck>();
            Players = new Dictionary<string, string>();
            cardFactory = CardFactoryHelper.GetFactory(factoryType);
            topCard = cardFactory.GenerateCard();
            isStarted = false;
            currentPlayerIndex = 0;
            direction = 1;
        }

        public void Start()
        {
            isStarted = true;
            foreach (var player in Players.Values)
            {
                PlayerDeck deck = new PlayerDeck(player, cardFactory);
                PlayerDecks.Add(deck);
            }

        }

        public void NextPlayer()
        {
            currentPlayerIndex = (currentPlayerIndex + direction + PlayerDecks.Count) % PlayerDecks.Count;
        }

        public UnoCard GenerateCard()
        {
            return cardFactory.GenerateCard();
        }
    }
}