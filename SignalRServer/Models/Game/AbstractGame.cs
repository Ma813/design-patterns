using SignalRServer.Models.CardPlacementStrategies;

namespace SignalRServer.Models.Game;

public abstract class AbstractGame
{
    public string RoomName { get; set; }
    public List<PlayerDeck> PlayerDecks { get; set; }
    public Dictionary<string, string> Players { get; set; } // {connectionId: username}
    public UnoCard TopCard { get; set; }
    public bool IsStarted { get; set; }
    public int CurrentPlayerIndex { get; set; }
    public int Direction { get; set; } // 1 for clockwise, -1 for counter-clockwise
    public List<BotClient> Bots { get; set; } = new List<BotClient>();
    protected ICardPlacementStrategy CardPlacementStrategy { get; set; }


    protected AbstractGame(string roomName)
    {
        RoomName = roomName;
        PlayerDecks = [];
        Players = [];
        TopCard = UnoCard.GenerateCard();
        IsStarted = false;
        CurrentPlayerIndex = 0;
        Direction = 1;
        CardPlacementStrategy = new UnoPlacementStrategy();
    }

    public ICardPlacementStrategy GetPlacementStrategy()
    {
        return CardPlacementStrategy;
    }

    public void SetPlacementStrategy(ICardPlacementStrategy strategy)
    {
        CardPlacementStrategy = strategy;
    }
    public abstract void Start();
    public abstract void End();
    public abstract void DrawCard(string username);
    public abstract string PlayCard(string username, UnoCard card);
    protected abstract void NextPlayer();

    public async Task NotifyBots()
    {
        foreach (var bot in Bots)
        {
            await bot.getNotified();
        }
    }

    public string AddBot()
    {
        var bot = new BotClient($"Bot{Bots.Count + 1}", this);
        Bots.Add(bot);
        return bot.userName;
    }
}
