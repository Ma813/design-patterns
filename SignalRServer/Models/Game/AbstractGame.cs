using System.Runtime.CompilerServices;
using Microsoft.AspNetCore.SignalR;
using SignalRServer.Card;
using SignalRServer.Models.CardPlacementStrategies;
using SignalRServer.Models.Commands;
using SignalRServer.Models.Game.States;
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

    // Add state management
    private IGameState _currentState;

    // TODO: see if changing this from protected to public causes any issues
    public ICardPlacementStrategy CardPlacementStrategy { get; set; }
    public IUnoThemeFactory ThemeFactory { get; set; }
    public Dictionary<string, int> PlacedCardCount { get; set; }

    protected PlayerTurnContainer? _turnContainer;
    protected PlayerTurnIterator? _turnIterator;
    // Validation chain
    public List<ICardPlayValidator> validation = new List<ICardPlayValidator>{
        new NullValidator(), new TurnValidator(), new CardOwnershipValidator(), new PlacementStrategyValidator()};

    protected AbstractGame(string roomName = "DefaultRoom")
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

        _currentState = new InactiveState();
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
            PlayerDeck deck = new(player.Value, Generator, client: clients?.Client(player.Key));
            PlayerDecks.Add(deck);
        }
        
        // *** INITIALIZE TURN ITERATOR ***
        InitializeTurnIterator();

        if (_currentState is LobbyState)
        {
            TransitionToState(new PlayingState());
        }
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

    public void TransitionToState(IGameState newState)
    {
        if (_currentState.CanTransitionTo(newState))
        {
            Console.WriteLine($"[{RoomName}] State transition: {_currentState.GetStateName()} -> {newState.GetStateName()}");
            _currentState = newState;
        }
        else
        {
            Console.WriteLine($"[{RoomName}] Invalid state transition attempted: {_currentState.GetStateName()} -> {newState.GetStateName()}");
        }
    }
    
    public string GetCurrentStateName() => _currentState.GetStateName();
    
    public IGameState GetCurrentState() => _currentState;
    
    // State-based wrapper methods
    public async Task<string> JoinRoomWithState(string userName, string connectionId)
    {
        var result = await _currentState.HandleJoinRoom(this, userName, connectionId);
        
        // Handle state transitions
        if (_currentState is InactiveState && Players.Count > 0)
        {
            TransitionToState(new LobbyState());
        }
        
        return result;
    }
    
    public async Task<string> StartGameWithState()
    {
        var result = await _currentState.HandleStartGame(this);
        
        Console.WriteLine($"[{RoomName}] StartGameWithState result: {result}, Current state: {_currentState.GetStateName()}");
        
        // Handle state transitions
        if (result == "OK" && _currentState is LobbyState)
        {
            TransitionToState(new PlayingState());
        }
        else if (result == "Ready for new game" && _currentState is GameOverState)
        {
            TransitionToState(new LobbyState());
            Console.WriteLine($"[{RoomName}] Game reset complete, now in lobby state");
        }
        
        return result;
    }
    
    public async Task<string> DrawCardWithState(string userName)
    {
        return await _currentState.HandleDrawCard(this, userName);
    }
    
    public async Task<string> PlayCardWithState(string userName, int cardIndex)
    {
        var result = await _currentState.HandlePlayCard(this, userName, cardIndex);
        
        // Handle state transitions
        if (result == "WIN")
        {
            TransitionToState(new GameOverState());
            Console.WriteLine($"[{RoomName}] Game ended, transitioning to GameOver state");
        }
        
        return result;
    }
    
    // Add method to properly reset game state
    public void ResetGameState()
    {
        Console.WriteLine($"[{RoomName}] Resetting game state");
        
        // Clear game data but keep players
        var playerUsernames = Players.Values.ToList();
        var playerConnections = Players.Keys.ToList();
        
        // Reset game state
        PlayerDecks.Clear();
        CurrentPlayerIndex = 0;
        IsStarted = false;
        
        // Keep players for next game
        Players.Clear();
        for (int i = 0; i < playerUsernames.Count; i++)
        {
            Players[playerConnections[i]] = playerUsernames[i];
        }
        
        Console.WriteLine($"[{RoomName}] Game reset complete. Players retained: {Players.Count}");
    }
    
    // Add method to check current state type
    public bool IsInState<T>() where T : IGameState
    {
        return _currentState is T;
    }
}
