//This class represents a player client in the SignalR server context.
//It is a hub for managing player connections

using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using SignalRServer.AnnoyingEffects;
using SignalRServer.Card;
using SignalRServer.Expressions;
using SignalRServer.Hubs;
using SignalRServer.Model.Chat.Colleagues;
using SignalRServer.Models.CardPlacementStrategies;
using SignalRServer.Models.Chat;
using SignalRServer.Models.Chat.Colleagues;
using SignalRServer.Models.Game;
using SignalRServer.Models.ThemeFactories;
using SignalRServer.Visitors;
using SignalRServer.Logging;

namespace SignalRServer.Models;

public class Facade
{
    private static Facade? _instance = null;
    IHubContext<PlayerHub> _hubContext;

    public static Facade GetInstance(IHubContext<PlayerHub> hubContext)
    {
        if (_instance == null)
        {
            _instance = new Facade(hubContext);
        }
        return _instance;
    }

    private static readonly Dictionary<string, string> UserRooms = new Dictionary<string, string>();

    private readonly Dictionary<string, AbstractGame> Games = new Dictionary<string, AbstractGame>(); // {roomName: Game}

    AbstractGameCreator gameFactory = new GameCreator();

    private static readonly Dictionary<string, string> UsernameToConnectionId = new Dictionary<string, string>();

    private readonly Dictionary<string, ChatMediator> _chatMediators = new();

    private static readonly SoundEffectAdaptee annoyingSoundAdaptee = new SoundEffectAdaptee();
    AnnoyingFlashbang annoyingFlashbang = new AnnoyingFlashbang();
    AnnoyingSoundEffect annoyingSoundEffect;

    private readonly Dictionary<string, SystemColleague> _systemColleagues = new();

    private readonly LoggingChain _loggingChain = new LoggingChain();

    public Facade(IHubContext<PlayerHub> hubContext)
    {
        _hubContext = hubContext;
        annoyingSoundEffect = new AnnoyingSoundEffect(annoyingSoundAdaptee);
    }

    public async Task JoinRoom(string roomName, string userName, IHubCallerClients Clients, HubCallerContext Context, IGroupManager Groups, int botAmount = 0, string gameMode = "Classic", string cardPlacementStrategy = "UnoPlacementStrategy", string theme = "Classic")
    {
        _loggingChain.Log(LogLevelCustom.Info, $"Player {userName} attempting to join room {roomName}", "RoomManager");
        var connectionId = Context.ConnectionId;

        // Remove user from previous room if any
        if (UserRooms.ContainsKey(connectionId))
        {
            await Groups.RemoveFromGroupAsync(connectionId, UserRooms[connectionId]);
        }

        if (UserRooms.ContainsKey(connectionId) && UserRooms[connectionId] == roomName) //player is already in room
        {
            return;
        }

        AbstractGame game;
        if (Games.ContainsKey(roomName)) 
        {
            game = Games[roomName];
        }
        else
        {
            game = gameFactory.CreateGame(gameMode, roomName);
            Games[roomName] = game;
        }

        UsernameToConnectionId[userName] = connectionId;

        // Use state-based join validation
        string stateResult = await game.JoinRoomWithState(userName, connectionId);
        if (stateResult.Contains("already") || stateResult.Contains("Cannot"))
        {
            _loggingChain.Log(LogLevelCustom.Error, $"Failed to create or join room {roomName} for player {userName}", "RoomManager");
            await Clients.Caller.SendAsync("Error", stateResult);
            return;
        }

        // Your existing placement strategy logic
        ICardPlacementStrategy strategy = cardPlacementStrategy switch
        {
            "AdjacentNumberPlacementStrategy" => new AdjacentNumberPlacementStrategy(),
            "ColorOnlyPlacementStrategy" => new ColorOnlyPlacementStrategy(),
            "NumberOnlyPlacementStrategy" => new NumberOnlyPlacementStrategy(),
            "UnoPlacementStrategy" => new UnoPlacementStrategy(),
            _ => throw new NotImplementedException()
        };

        IUnoThemeFactory themeFactory = theme.ToLower() switch
        {
            "halloween" => new HalloweenThemeFactory(),
            _ => new ClassicThemeFactory()
        };

        game.SetPlacementStrategy(strategy);
        if (game.Players.Count == 1) game.ThemeFactory = themeFactory; // First player sets theme

        // Add user to new room
        UserRooms[connectionId] = roomName;
        await Groups.AddToGroupAsync(connectionId, roomName);

        // Your existing bot logic
        var usernames = game.Players.Values.ToArray();
        if (botAmount > 0)
        {
            string botName = game.AddFirstBot();
            usernames = usernames.Append(botName).ToArray();
            game.Players[$"bot-{botName}"] = botName;
        }
        for (int i = 2; i <= botAmount; i++)
        {
            string botName = game.AddNextBots();
            usernames = usernames.Append(botName).ToArray();
            game.Players[$"bot-{botName}"] = botName;
        }

        await Clients.Group(roomName).SendAsync("UserJoined", usernames);
        await Clients.Group(roomName).SendAsync("GameStateChanged", game.GetCurrentStateName());
        
        SystemMessages.UserJoined(game, connectionId, userName, roomName, Clients).Wait();

        if (_systemColleagues.TryGetValue(roomName, out var systemColleague)) 
            await systemColleague.AnnouncePlayerJoined(userName);

        // Your existing theme logic
        var themeInfo = new
        {
            cardDesign = game.ThemeFactory.CreateCardDesign().GetDesignInfo(),
            background = game.ThemeFactory.CreateBackground().GetBackgroundInfo(),
            sound = game.ThemeFactory.CreateSoundEffect().GetSoundInfo()
        };
        await Clients.Group(roomName).SendAsync("ThemeSelected", themeInfo);
    }

    public async Task StartGame(string roomName, string userName, IHubCallerClients? Clients, string connectionId = "bot")
    {
        Console.WriteLine($"[{roomName}] StartGame called by {userName}");
        
        AbstractGame game = Games[roomName];
        
        Console.WriteLine($"[{roomName}] Current game state before start: {game.GetCurrentStateName()}");
        
        // Check state before starting
        string stateResult = await game.StartGameWithState();
        
        Console.WriteLine($"[{roomName}] State result: {stateResult}");
        
        if (stateResult.Contains("Need") || stateResult.Contains("Cannot"))
        {
            if (Clients != null)
            {
                await Clients.Caller.SendAsync("Error", stateResult);
            }
            Console.WriteLine($"[{roomName}] StartGame failed: {stateResult}");
            return;
        }
        
        // Handle the case where we're coming from GameOver state
        if (stateResult == "Ready for new game")
        {
            Console.WriteLine($"[{roomName}] Game was reset, now in {game.GetCurrentStateName()} state");
            
            // Try starting again now that we're in lobby state
            stateResult = await game.StartGameWithState();
            Console.WriteLine($"[{roomName}] Second start attempt result: {stateResult}");
            
            if (stateResult != "OK")
            {
                if (Clients != null)
                {
                    await Clients.Caller.SendAsync("Error", stateResult);
                }
                return;
            }
        }
        
        // Only proceed if we got OK or if we successfully transitioned
        if (stateResult == "OK" || game.GetCurrentStateName() == "Playing")
        {
            Console.WriteLine($"[{roomName}] Starting game logic, current state: {game.GetCurrentStateName()}");
            
            // Your existing start game logic
            game.Start(Clients);

            foreach (var player in game.Players)
            {
                GameForSending gameForSeding = new GameForSending(game, player.Value);
                await Clients.Client(player.Key).SendAsync("GameStarted", gameForSeding);
            }

            await SetupChatMediator(roomName, game, Clients);

            SystemMessages.GameStarted(game, userName, connectionId, Clients).Wait();
            if (_systemColleagues.TryGetValue(roomName, out var systemColleague)) 
                await systemColleague.AnnounceGameStarted();
            
            await Clients.Group(roomName).SendAsync("GameStateChanged", game.GetCurrentStateName());
            Console.WriteLine($"[{roomName}] Game started successfully");
        }
        else
        {
            Console.WriteLine($"[{roomName}] Unexpected state result: {stateResult}");
            if (Clients != null)
            {
                await Clients.Caller.SendAsync("Error", "Failed to start game");
            }
        }
    }

    public async Task<string> DrawCard(string roomName, string userName, IHubCallerClients<IClientProxy>? Clients, string connectionId = "bot")
    {
        if (Clients == null)
        {
            Clients = new NullClients();
        }

        AbstractGame game = Games[roomName];
        
        // Use state-based draw validation
        string stateResult = await game.DrawCardWithState(userName);
        if (stateResult != "OK")
        {
            await Clients.Caller.SendAsync("Error", stateResult);
            return stateResult;
        }
        
        // Your existing logic
        SystemMessages.CardDrawn(game, userName, connectionId, Clients).Wait();
        await notifyPlayers(game);
        return "OK";
    }

    public async Task<string> PlayCard(string roomName, string userName, int cardIndex, IHubCallerClients<IClientProxy>? Clients, string connectionId = "bot")
    {
        if (Clients == null)
        {
            Clients = new NullClients();
        }

        AbstractGame game = Games[roomName];

        // Use state-based play validation
        string stateResult = await game.PlayCardWithState(userName, cardIndex);
        
        if (stateResult == "Player not found")
        {
            await Clients.Caller.SendAsync("Error", "Player not found");
            return "Error: Player not found";
        }
        
        if (stateResult == "WIN")
        {
            SystemMessages.CardPlayed(game, userName, connectionId, Clients, true).Wait();
            if (_systemColleagues.TryGetValue(roomName, out var systemColleague)) 
                await systemColleague.AnnounceWinner(userName);
            await _hubContext.Clients.Group(roomName).SendAsync("GameEnded", $"{userName} has won the game!");
            await _hubContext.Clients.Group(roomName).SendAsync("GameStateChanged", game.GetCurrentStateName());
            return "WIN";
        }
        
        if (stateResult != "OK")
        {
            await Clients.Caller.SendAsync("Error", stateResult);
            return "Error: " + stateResult;
        }
        
        // Your existing success logic
        SystemMessages.CardPlayed(game, userName, connectionId, Clients, false).Wait();
        
        var playerDeck = game.PlayerDecks.FirstOrDefault(pd => pd.Username == userName);
        if(playerDeck?.Cards.Count == 1)
        {
            if (_systemColleagues.TryGetValue(roomName, out var systemColleague)) 
                await systemColleague.AnnounceUnoCall(userName);
        }

        await notifyPlayers(game);
        return "OK";
    }

    // Add new method to get game state
    public string GetGameState(string roomName)
    {
        if (Games.ContainsKey(roomName))
        {
            return Games[roomName].GetCurrentStateName();
        }
        return "Room not found";
    }

    public async Task UndoCard(string roomName, string userName, IHubCallerClients<IClientProxy>? Clients)
    {
        if (Clients == null)
        {
            Clients = (IHubCallerClients<IClientProxy>)new NullClients();
        }

        AbstractGame game = Games[roomName];
        string result = game.UndoCard(userName);
        if (result != "OK")
        {
            await Clients.Caller.SendAsync("Error", result);
            return;
        }

        await notifyPlayers(game);
    }

    public async Task AnnoyPlayers(string roomName, string message, IHubCallerClients<IClientProxy> Clients, HubCallerContext Context)
    {
        IClientProxy playerGroup = Clients.Group(roomName);

        IAnnoyingEffects annoyingEffect;

        switch (message)
        {
            case "flashbang":
                annoyingEffect = annoyingFlashbang;
                break;
            case "soundeffect":
                annoyingEffect = annoyingSoundEffect;
                break;
            default:
                return;
        }

        Dictionary<string, IClientProxy> players = new Dictionary<string, IClientProxy>();
        foreach (var player in UsernameToConnectionId)
        {
            if (UserRooms.TryGetValue(player.Value, out var playerRoom) && playerRoom == roomName)
            {
                players[player.Key] = Clients.Client(player.Value);
            }
        }

        string callerUsername = UsernameToConnectionId.FirstOrDefault(x => x.Value == Context.ConnectionId).Key;
        await annoyingEffect.AnnoyAll(players, Clients.Caller, callerUsername: callerUsername);
    }

    public async Task AnnoyPlayer(string roomName, string player, string message, IHubCallerClients<IClientProxy> Clients, HubCallerContext Context)
    {
        if (!UsernameToConnectionId.ContainsKey(player))
        {
            return;
        }
        string playerId = UsernameToConnectionId[player];
        IClientProxy singlePlayer = Clients.Client(playerId);

        IAnnoyingEffects annoyingEffect;

        switch (message)
        {
            case "flashbang":
                annoyingEffect = annoyingFlashbang;
                break;
            case "soundeffect":
                annoyingEffect = annoyingSoundEffect;
                break;
            default:
                return;
        }

        string callerUsername = UsernameToConnectionId.FirstOrDefault(x => x.Value == Context.ConnectionId).Key;

        await annoyingEffect.Annoy(singlePlayer, Clients.Caller, player, callerUsername);
    }

    public async Task ToggleMutePlayer(string roomname, string player, IHubCallerClients Clients, HubCallerContext Context)
    {
        if (!UsernameToConnectionId.ContainsKey(player) && player != "SYSTEM")
        {
            return;
        }

        System.Console.WriteLine("ToggleMutePlayer: " + player);

        if(player != "SYSTEM"){
            string playerId = UsernameToConnectionId[player];
            IClientProxy muted = Clients.Client(playerId);
            IClientProxy muting = Clients.Caller;
        }

        string username = UsernameToConnectionId.FirstOrDefault(x => x.Value == Context.ConnectionId).Key;

        await annoyingSoundAdaptee.ToggleMutePlayer(player, username);
        _chatMediators[roomname].Mute(player, username);
    }

    public async Task SendTextMessage(string roomName, string sender, string text)
    {
        Message message = new Message(sender, text);
        await message.SendMessageAsync(_hubContext.Clients.Group(roomName));
    }

    public string InterpretExpression(IExpression expression, string command, HubCallerContext Context, IHubCallerClients clients, IGroupManager groups)
    {
        return expression.Interpret(command, Context, clients, groups);
    }


    public async Task notifyPlayers(AbstractGame game)
    {
        foreach (var player in game.Players)
        {

            PlayerDeck playerDeck = game.PlayerDecks.FirstOrDefault(pd => pd.Username == player.Value);

            playerDeck.Accept(new LoggerVisitor());
            playerDeck.Accept(new ScoreVisitor());
            playerDeck.Accept(new CardsVisitor());

            GameForSending gameForSending = new GameForSending(game, player.Value);
            await _hubContext.Clients.Client(player.Key).SendAsync("GameStatus", gameForSending);
        }

        foreach (var bot in game.Bots)
        {
            await bot.getNotified();
        }
    }

    public async Task NextPlayer(string roomName, string actionType, IHubCallerClients<IClientProxy>? Clients)
    {
        if (Clients == null)
        {
            Clients = (IHubCallerClients<IClientProxy>)new NullClients();
        }
        AbstractGame game = Games[roomName];
        game.NextPlayer((Action)int.Parse(actionType));
        await notifyPlayers(game);
    }
    public AbstractGame GetGame(string roomName)
    {
        return Games.ContainsKey(roomName) ? Games[roomName] : null;
    }
    public async Task JoinRoomThroughDirector(string roomName, string userName, string builderType, IHubCallerClients Clients, HubCallerContext Context, IGroupManager Groups)
    {
        try
        {
            IGameBuilder builder = builderType switch
            {
                "Classic" => new ClassicUnoGameBuilder(this),
                "Endless" => new EndlessAdjacentGameBuilder(this),
                _ => throw new ArgumentException("Unknown builder type")
            };

            var director = new GameDirector(builder);
            var game = await director.ConstructAsync(roomName, userName, Clients, Context, Groups);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"JoinRoomThroughDirector error: {ex}");
            throw; // rethrow to see it in frontend HubException
        }
    }

    public AbstractGame? GetGameByConnection(HubCallerContext context)
    {
        string connectionId = context.ConnectionId;

        if (UserRooms.TryGetValue(connectionId, out string roomName) && Games.ContainsKey(roomName))
        {
            return Games[roomName];
        }

        return null; // user is not in a game
    }

    public async Task SetupChatMediator(string roomName, AbstractGame game, IHubCallerClients clients)
    {
        var mediator = new ChatMediator(roomName);

        foreach(var player in game.Players.Where(p => !p.Key.StartsWith("bot-")))
        {
            var humanColleague = new HumanPlayerColleague(
                player.Value,
                player.Key,
                clients.Client(player.Key)
            );

            mediator.Register(humanColleague);
        }

        foreach(var bot in game.Bots)
        {
            var botColleague = new BotPlayerColleague(bot.UserName);
            mediator.Register(botColleague);
            await botColleague.SendRandomMessage(roomName);
        }

        // Register system colleague and store reference
        var systemColleague = new SystemColleague(_hubContext, roomName);
        mediator.Register(systemColleague);
        _systemColleagues[roomName] = systemColleague;

        _chatMediators[roomName] = mediator;
    }

    public async Task SendTextMessageThroughMediator(string roomName, string sender, string text)
    {
        if(_chatMediators.TryGetValue(roomName, out var mediator))
        {
            //System.Console.WriteLine("SendTextMessageThroughMediator");
            await mediator.SendMessage(sender, text, roomName);
        }
    }
}
