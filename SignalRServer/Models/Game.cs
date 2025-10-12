namespace SignalRServer.Models;

public class Game
{
    public List<PlayerDeck> PlayerDecks { get; set; }
    public Dictionary<string, string> Players { get; set; } // Players Dictionary - key is connectionId, value is username
    public BaseCard topCard { get; set; }
    public bool isStarted { get; set; }
    public int currentPlayerIndex { get; set; }
    public int direction { get; set; } // Describes game direction 1 for clockwise, -1 for counter-clockwise (To swap direction, just multiply by -1)

    public Game()
    {
        PlayerDecks = [];
        Players = [];

        var random = new Random();
        topCard = new NumberCard(((Colors)random.Next(0, 5)).ToString(), random.Next(0, 10)); // Initial top card generation

        isStarted = false;
        currentPlayerIndex = 0;
        direction = 1;
    }

    public void Start()
    {
        isStarted = true;
        foreach (var player in Players.Values)
        {
            PlayerDeck deck = new(player);
            PlayerDecks.Add(deck);
        }
    }

    public void NextPlayer()
    {
        currentPlayerIndex = (currentPlayerIndex + direction + PlayerDecks.Count) % PlayerDecks.Count;
    }
}