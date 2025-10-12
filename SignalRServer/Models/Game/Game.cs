using SignalRServer.Helpers;

namespace SignalRServer.Models;

public class Game : AbstractGame { }

public abstract class AbstractGame
{
    public List<PlayerDeck> PlayerDecks { get; set; }
    public Dictionary<string, string> Players { get; set; } // Players Dictionary - key is connectionId, value is username
    public BaseCard TopCard { get; set; }
    public bool IsStarted { get; set; }
    public int CurrentPlayerIndex { get; set; }
    public int Direction { get; set; } // Describes game direction 1 for clockwise, -1 for counter-clockwise (To swap direction, just multiply by -1)

    protected static readonly Logger logger = Logger.GetInstance();

    protected AbstractGame()
    {
        PlayerDecks = [];
        Players = [];

        var random = new Random();
        TopCard = new NumberCard(((Colors)random.Next(0, 5)).ToString(), random.Next(0, 10)); // Initial top card generation

        IsStarted = false;
        CurrentPlayerIndex = 0;
        Direction = 1;
    }

    public void Start()
    {
        IsStarted = true;
        foreach (var player in Players.Values)
        {
            PlayerDeck deck = new(player);
            PlayerDecks.Add(deck);
        }
        logger.LogInfo("Game started with players: " + string.Join(", ", Players.Values));
    }

    public virtual void End()
    {
        IsStarted = false;
        PlayerDecks.Clear();
        CurrentPlayerIndex = 0;
        Direction = 1;
        logger.LogInfo("Game ended.");
    }

    public virtual void DrawCard(string username)
    {
        var playerDeck = PlayerDecks.FirstOrDefault(pd => pd.Username == username);
        if (playerDeck != null)
        {
            var random = new Random();
            var card = new NumberCard(((Colors)random.Next(0, 5)).ToString(), random.Next(0, 10));
            playerDeck.AddCard(card);
        }
        NextPlayer();
        logger.LogInfo($"{username} drew a card.");
    }

    public string PlayCard(string username, BaseCard card)
    {
        CurrentPlayerIndex = (CurrentPlayerIndex + Direction + PlayerDecks.Count) % PlayerDecks.Count;
        var playerDeck = PlayerDecks.FirstOrDefault(pd => pd.Username == username);

        if (playerDeck == null) return "Player not found";
        if (playerDeck != PlayerDecks[CurrentPlayerIndex]) return "Not your turn";
        if (!card.CanPlay(TopCard)) return "Card cannot be played on top of current top card";

        logger.LogInfo($"{username} played {card.Color + ' ' + card.Name} on top of {TopCard.Color + ' ' + TopCard.Name}");

        playerDeck.RemoveCard(card);
        TopCard = card;
        if (playerDeck.Count == 0)
        {
            End();
            return "WIN";
        }
        NextPlayer();
        return "OK";
    }

    protected void NextPlayer()
    {
        CurrentPlayerIndex = (CurrentPlayerIndex + Direction + PlayerDecks.Count) % PlayerDecks.Count;
    }
}