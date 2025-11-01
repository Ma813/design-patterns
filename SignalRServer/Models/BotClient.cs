using System;
using System.Threading.Tasks;

namespace SignalRServer.Models
{
    public class BotClient
    {
        public string userName;
        AbstractGame game;
        public Facade facade;
        public BotClient(string userName, AbstractGame game)
        {
            this.userName = userName;
            this.game = game;
            this.facade = Facade.GetInstance(null!); // Hacky way to get Facade instance
            // It's okay, because the GameHub will always create the Facade first with a valid hub context
        }

        public async Task getNotified()
        {
            if (game.Players.Values.ElementAt(game.CurrentPlayerIndex) == userName)
            {
                // Simple bot logic: draw a card if no playable card, else play the first playable card
                var playerDeck = game.PlayerDecks.First(pd => pd.Username == userName);
                // TODO Change logic to find a playable card to match ruleset
                var playableCard = playerDeck.Cards.FirstOrDefault(card =>
                    card.Color == game.TopCard.Color ||
                    card.Digit == game.TopCard.Digit ||
                    card.Color == "Wild");

                if (playableCard != null)
                {
                    Console.WriteLine($"{userName} (Bot) plays {playableCard.Color} {playableCard.Digit}");
                    await facade.PlayCard(game.RoomName, userName, playableCard, null);
                }
                else
                {
                    Console.WriteLine($"{userName} (Bot) has no playable card and draws a card.");
                    await facade.DrawCard(game.RoomName, userName, null);
                }
                System.Console.WriteLine($"{userName} has: {string.Join(", ", playerDeck.Cards.Select(c => c.Color + " " + c.Digit))}");        
            }

        }

    }
}