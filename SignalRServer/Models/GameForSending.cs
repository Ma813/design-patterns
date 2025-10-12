namespace SignalRServer.Models
{
    public class GameForSending
    {
        public Dictionary<string, int> PlayerAmounts { get; set; }
        public UnoCard topCard { get; set; }
        public string currentPlayer { get; set; }
        public int direction { get; set; }
        public PlayerDeck? PlayerDeck { get; set; } // The deck of the player requesting the game state
        public int redCardCount { get; set; }

        public GameForSending(Game game, string userName)
        {
            PlayerAmounts = new Dictionary<string, int>();
            foreach (PlayerDeck pDeck in game.PlayerDecks)
            {
                PlayerAmounts[pDeck.Username] = pDeck.Count;
            }
            topCard = game.topCard;
            currentPlayer = game.PlayerDecks[game.currentPlayerIndex].Username;
            direction = game.direction;
            PlayerDeck = game.PlayerDecks.FirstOrDefault(pd => pd.Username == userName);
            redCardCount = game.PlayerDecks[game.currentPlayerIndex].redCardCount;
        }
    }
}