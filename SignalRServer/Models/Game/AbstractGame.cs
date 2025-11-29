using System.Runtime.CompilerServices;
using Microsoft.AspNetCore.SignalR;
using SignalRServer.Card;
using SignalRServer.Models.CardPlacementStrategies;
using SignalRServer.Models.Commands;
using SignalRServer.Models.ThemeFactories;

namespace SignalRServer.Models.Game;

public abstract class AbstractGame : ISubject
{
    public string RoomName { get; set; }
    public List<PlayerDeck> PlayerDecks { get; set; }
    public Dictionary<string, string> Players { get; set; } // {connectionId: username}
    public UnoCard TopCard { get; set; }
    public bool IsStarted { get; set; }
    public int CurrentPlayerIndex { get; set; }
    public int Direction { get; set; } // 1 for clockwise, -1 for counter-clockwise
    public List<IBotClientPrototype> Bots { get; set; } = [];

    // TODO: see if changing this from protected to public causes any issues
    public ICardPlacementStrategy CardPlacementStrategy { get; set; }
    public IUnoThemeFactory ThemeFactory { get; set; }
    public Dictionary<string, int> PlacedCardCount { get; set; }

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
        PlacedCardCount = [];

        AttachObservers();
    }

    public ICardPlacementStrategy GetPlacementStrategy()
    {
        return CardPlacementStrategy;
    }

    public void SetPlacementStrategy(ICardPlacementStrategy strategy)
    {
        CardPlacementStrategy = strategy;
    }
    public abstract void Start(IHubCallerClients? clients = null);
    public abstract void End();
    public abstract void DrawCard(string username);
    public abstract string UndoCard(string username);
    public abstract void NextPlayer(Action action);
    public abstract void NextDrawCard();

    public async Task NotifyBots()
    {
        foreach (var bot in Bots)
        {
            await bot.getNotified();
        }
    }

    public string AddFirstBot()
    {
        var bot = new BotClient($"Bot{Bots.Count + 1}", this);
        Bots.Add(bot);
        return bot.UserName;
    }

    public string AddNextBots()
    {
        var botPrototype = Bots[0];
        var newBot = botPrototype.Clone($"Bot{Bots.Count + 1}");
        Bots.Add(newBot);
        return newBot.UserName;
    }

    public void AttachObservers()
    {
        Add(new CardCountUpdater());
    }

    public void SetCardCount(Dictionary<string, int> cardCount)
    {
        PlacedCardCount = cardCount;
    }

    public string PlayCard(string username, UnoCard card)
    {
        var playerDeck = PlayerDecks.FirstOrDefault(pd => pd.Username == username);

        NullValidator nv = new NullValidator();
        TurnValidator tv = new TurnValidator();
        CardOwnershipValidator cov = new CardOwnershipValidator();
        PlacementStrategyValidator psv = new PlacementStrategyValidator();

        nv.SetNext(tv);
        tv.SetNext(cov);
        cov.SetNext(psv);

        BaseCardPlayValidator validator = nv;

        var result = validator.Validate(playerDeck, card, this);
        if(result != "OK") return result;
        
        // if (playerDeck == null) return "Player not found";
        // if (playerDeck != PlayerDecks[CurrentPlayerIndex]) return "Not your turn";
        // if (!card.CanPlayOn(TopCard, CardPlacementStrategy)) return "Card cannot be played on top of current top card";

        playerDeck.ExecuteCommand(new PlayCardCommand(this, playerDeck, card, CardPlacementStrategy));
        if (playerDeck.Count == 0)
        {
            End();
            return "WIN";
        }
        return "OK";
    }
}
