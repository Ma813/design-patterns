using SignalRServer.Helpers;

namespace SignalRServer.Models;

public class Game : AbstractGame
{
    public Game(CardGeneratingMode cardGeneratingMode, StrategyType placementStrategy) : base(cardGeneratingMode, placementStrategy)
    {
    }
}

public abstract class AbstractGame
{
    public List<PlayerDeck> PlayerDecks { get; set; }
    public Dictionary<string, string> Players { get; set; } // Players Dictionary - key is connectionId, value is username
    public BaseCard TopCard { get; set; }
    public bool IsStarted { get; set; }
    public int CurrentPlayerIndex { get; set; }
    public int Direction { get; set; } // Describes game direction 1 for clockwise, -1 for counter-clockwise (To swap direction, just multiply by -1)

    protected ICardPlacementStrategy cardPlacementStrategy { get; set; }
    protected ICardFactory cardFactory;

    protected static readonly Logger logger = Logger.GetInstance();

    protected AbstractGame(CardGeneratingMode cardGeneratingMode, StrategyType placementStrategy)
    {
        PlayerDecks = [];
        Players = [];

        cardFactory = CardFactoryCreator.GetFactory(cardGeneratingMode);
        TopCard = cardFactory.GenerateCard();

        cardPlacementStrategy = StrategyCreator.GetStrategy(placementStrategy);

        IsStarted = false;
        CurrentPlayerIndex = 0;
        Direction = 1;
    }

    public void SetPlacementStrategy(ICardPlacementStrategy strategy)
    {
        cardPlacementStrategy = strategy;
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
        if (playerDeck != default)
        {
            var random = new Random();
            var card = cardFactory.GenerateCard();
            playerDeck.AddCard(card);
        }
        NextPlayer();
        logger.LogInfo($"{username} drew a card.");
    }

    public string PlayCard(string username, BaseCard card)
    {
        var playerDeck = PlayerDecks.FirstOrDefault(pd => pd.Username == username);

        if (playerDeck == default) return "Player not found";
        if (playerDeck.Username != PlayerDecks[CurrentPlayerIndex].Username) return "Not your turn";
        if (!card.CanPlay(TopCard, cardPlacementStrategy)) return "Card cannot be played on top of current top card";

        logger.LogInfo($"{username} played {card.Color + ' ' + card.Name} on top of {TopCard.Color + ' ' + TopCard.Name}");

        playerDeck.RemoveCard(card);
        card.Play(this);
        TopCard = card;
        if (playerDeck.Count == 0)
        {
            End();
            return "WIN";
        }
        NextPlayer();
        return "OK";
    }

    public void NextPlayer()
    {
        CurrentPlayerIndex = (CurrentPlayerIndex + Direction + PlayerDecks.Count) % PlayerDecks.Count;
    }
}