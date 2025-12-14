using System.Runtime.CompilerServices;
using Microsoft.AspNetCore.SignalR;
using SignalRServer.Card;
using SignalRServer.Models.CardPlacementStrategies;
using SignalRServer.Models.Commands;
using SignalRServer.Models.Iterator;
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
    public readonly CardGenerator Generator;

    // TODO: see if changing this from protected to public causes any issues
    public ICardPlacementStrategy CardPlacementStrategy { get; set; }
    public IUnoThemeFactory ThemeFactory { get; set; }
    public Dictionary<string, int> PlacedCardCount { get; set; }

    protected PlayerTurnContainer? _turnContainer;
    protected PlayerTurnIterator? _turnIterator;
    // Validation chain
    public List<ICardPlayValidator> validation = new List<ICardPlayValidator>{
        new NullValidator(), new TurnValidator(), new CardOwnershipValidator(), new PlacementStrategyValidator()};

    protected AbstractGame(string roomName)
    {
        RoomName = roomName;
        PlayerDecks = [];
        Players = [];  
        Generator = new StandardCardGenerator(UnoCard.PossibleColors);
        TopCard = Generator.GenerateRandomCard();
        IsStarted = false;
        CurrentPlayerIndex = 0;
        Direction = 1;
        CardPlacementStrategy = new UnoPlacementStrategy();
        PlacedCardCount = [];

        AttachObservers();

        // set up validation chain
        for(int i = 0; i < validation.Count - 1; i++) validation[i].SetNext(validation[i+1]);
        validation[validation.Count - 1].SetNext(null);
    }

    public ICardPlacementStrategy GetPlacementStrategy()
    {
        return CardPlacementStrategy;
    }

    public void SetPlacementStrategy(ICardPlacementStrategy strategy)
    {
        CardPlacementStrategy = strategy;
    }

    protected void InitializeTurnIterator()
    {
        _turnContainer = new PlayerTurnContainer(PlayerDecks, Direction);
        _turnIterator = (PlayerTurnIterator)_turnContainer.CreateIterator();
    }

    public PlayerTurnIterator GetTurnIterator()
    {
        if (_turnIterator == null || _turnContainer == null)
        {
            InitializeTurnIterator();
        }
        return _turnIterator!;
    }

    public virtual void NextPlayerWithIterator()
    {
        var iterator = GetTurnIterator();
        
        // Update direction in aggregate if it changed (reverse card)
        _turnContainer!.SetDirection(Direction);
        
        // Move to next player
        iterator.Next();
        CurrentPlayerIndex = iterator.CurrentIndex;
    }

    public virtual void SkipNextPlayer()
    {
        var iterator = GetTurnIterator();
        _turnContainer!.SetDirection(Direction);
        iterator.Skip(2); // Skip to the player after next
        CurrentPlayerIndex = iterator.CurrentIndex;
    }

    public PlayerDeck PeekNextPlayer()
    {
        var iterator = GetTurnIterator();
        _turnContainer!.SetDirection(Direction);
        return iterator.PeekNext();
    }

    public void Start(IHubCallerClients? clients = null)
    {
        IsStarted = true;
        foreach (var player in Players)
        {
            PlayerDeck deck = new(player.Value, client: clients?.Client(player.Key));
            PlayerDecks.Add(deck);
        }
        
        // *** INITIALIZE TURN ITERATOR ***
        InitializeTurnIterator();
    }

    public abstract void End();
    public abstract void DrawCard(string username);
    public abstract string UndoCard(string username);
    public virtual void NextPlayer(Action action)
    {
        var iterator = GetTurnIterator();
        _turnContainer!.SetDirection(Direction);  // In case direction changed (reverse card)
        iterator.Next();
        CurrentPlayerIndex = iterator.CurrentIndex;
    }
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

        var result = validation.First().Validate(playerDeck, card, this);
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

        if (this is not DrawToMatchGame)
        {
            NextPlayer(Action.place);
        }
        return "OK";
    }
}
