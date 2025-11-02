using SignalRServer.Models.Game;

namespace SignalRServer.Models;

public class BotClient : IBotClientPrototype
{
    private static readonly Logger logger = Logger.GetInstance();
    public string UserName { get; private set; }
    private AbstractGame game;
    public Facade facade;

    private BotClient(string userName, AbstractGame game, Facade facade)
    {
        UserName = userName;
        this.game = game;
        this.facade = facade;
    }

    public BotClient(string userName, AbstractGame game)
    {
        UserName = userName;
        this.game = game;
        facade = Facade.GetInstance(null!); // Hacky way to get Facade instance
                                            // It's okay, because the GameHub will always create the Facade first with a valid hub context
    }

    public IBotClientPrototype Clone(string newUserName)
    {
        return new BotClient(newUserName, game, facade);
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
            // TODO Change logic to find a playable card to match ruleset
            var playableCardIndex = playerDeck.Cards.FindIndex(card =>
                card.CanPlayOn(game.TopCard, game.GetPlacementStrategy()));

            if (playableCardIndex != -1)
            {
                logger.LogInfo($"{UserName} (Bot) plays {playerDeck.Cards[playableCardIndex].Color} {playerDeck.Cards[playableCardIndex].Digit}");
                game.NextPlayer(Action.place);
                await facade.PlayCard(game.RoomName, UserName, playableCardIndex, null);

                // Brag about winning
                if (playerDeck.Cards.Count == 0)
                {
                    await facade.SendTextMessage(game.RoomName, UserName, "I win! Better luck next time humans!");
                }
                await facade.notifyPlayers(game);
            }
            else
            {
                logger.LogInfo($"{UserName} (Bot) has no playable card and draws a card.");
                game.NextPlayer(Action.draw);
                await facade.DrawCard(game.RoomName, UserName, null);
                await facade.notifyPlayers(game);
            }
            logger.LogInfo($"{UserName} has: {string.Join(", ", playerDeck.Cards.Select(c => c.Color + " " + c.Digit))}");
        }
    }
}