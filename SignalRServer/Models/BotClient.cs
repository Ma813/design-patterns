using System.Runtime.CompilerServices;
using SignalRServer.Models.Game;
using SignalRServer.Models.Iterator;
namespace SignalRServer.Models;

public class BotClient : IBotClientPrototype
{
    private static readonly Logger logger = Logger.GetInstance();
    private static int _instanceCounter = 0;
    private readonly int _instanceId; // Track instance number 
    public string UserName { get; private set; }
    public int DifficultyLevel { get; private set; }
    public string Strategy { get; private set; }
    private AbstractGame game;
    public Facade facade;

    private BotClient(string userName, AbstractGame game, Facade facade, int difficultyLevel, string strategy)
    {
        _instanceId = ++_instanceCounter;
        UserName = userName;
        this.game = game;
        this.facade = facade;
        DifficultyLevel = difficultyLevel;
        Strategy = strategy;
        logger.LogInfo($"Created BotClient #{_instanceId}: {UserName} (Memory: {GetMemoryInfo(this)})");
    }

    public BotClient(string userName, AbstractGame game, int difficultyLevel = 1, string strategy = "Basic")
    {
        _instanceId = ++_instanceCounter;
        UserName = userName;
        this.game = game;
        facade = Facade.GetInstance(null!); // Hacky way to get Facade instance 
                                            // It's okay, because the GameHub will always create the Facade first with a valid hub context 
        DifficultyLevel = difficultyLevel;
        Strategy = strategy;
        logger.LogInfo($"Created BotClient #{_instanceId}: {UserName} (Memory: {GetMemoryInfo(this)})");
    }

    public IBotClientPrototype Clone(string newUserName)
    {
        logger.LogInfo($"Cloning {UserName} -> {newUserName}");
        logger.LogInfo($"Original: {GetMemoryInfo(this)}");

        var clone = new BotClient(newUserName, game, facade, DifficultyLevel, Strategy);

        logger.LogInfo($"Clone: {GetMemoryInfo(clone)}");
        logger.LogInfo($"Same object? {ReferenceEquals(this, clone)}");
        return clone;
    }

    private static string GetMemoryInfo(object obj)
    {
        return $"Hash: 0x{obj.GetHashCode():X8}, RefEquals: {GetReferenceId(obj)}";
    }

    private static string GetReferenceId(object obj)
    {
        // This gives a consistent identifier for the same reference 

        return RuntimeHelpers.GetHashCode(obj).ToString("X8");
    }

    private async Task SendRandomMessageChanceAsync()
    {
        var random = new Random();
        if (random.NextDouble() < 0.2)
        {
            var messages = new[] {
                "Good luck everyone!",
                "Let's see what happens...",
                "I hope I get a good card!",
                "This is getting interesting.",
                "Watch out, here I come!",
                "Ur a communist!" //Should be censored 
            };
            var message = messages[random.Next(messages.Length)];
            await facade.SendTextMessage(game.RoomName, UserName, message);
        }
    }

    public async Task getNotified()
    {
        if (game.Players.Values.ElementAt(game.CurrentPlayerIndex) == UserName)
        {
            //sleep for a bit to simulate thinking
            await Task.Delay(1000);

            await SendRandomMessageChanceAsync();

            // Simple bot logic: draw a card if no playable card, else play the first playable card
            var playerDeck = game.PlayerDecks.First(pd => pd.Username == UserName);
            // // TODO Change logic to find a playable card to match ruleset
            // var playableCardIndex = playerDeck.Cards.FindIndex(card =>
            //     card.CanPlayOn(game.TopCard, game.GetPlacementStrategy()));

            var cardContainer = new PlayableCardContainer(
                playerDeck.Cards,
                game.TopCard,
                game.GetPlacementStrategy()
            );
            var cardIterator = (PlayableCardIterator)cardContainer.CreateIterator();

            if (!cardIterator.IsDone())
            {
                // Get the index of the first playable card
                int playableCardIndex = cardIterator.CurrentIndex;
                
                logger.LogInfo($"{UserName} (Bot) plays {playerDeck.Cards[playableCardIndex].Color} {playerDeck.Cards[playableCardIndex].Name}");
                logger.LogInfo($"  Found using iterator - playable indices: [{string.Join(", ", cardIterator.GetAllPlayableIndices())}]");
                
                game.NextPlayer(Action.place);
                await facade.PlayCard(game.RoomName, UserName, playableCardIndex, null);

                if (playerDeck.Cards.Count == 0)
                {
                    await facade.SendTextMessage(game.RoomName, UserName, "I win! Better luck next time humans!");
                }
                await facade.notifyPlayers(game);
            }
            else
            {
                // No playable card found by iterator
                logger.LogInfo($"{UserName} (Bot) has no playable card (iterator found 0 matches). Drawing a card.");
                game.NextPlayer(Action.draw);
                await facade.DrawCard(game.RoomName, UserName, null);
                await facade.notifyPlayers(game);
            }
            
            logger.LogInfo($"{UserName} has: {string.Join(", ", playerDeck.Cards.Select(c => c.Color + " " + c.Name))}");
        }
    }
}