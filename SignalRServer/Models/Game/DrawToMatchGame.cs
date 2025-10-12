namespace SignalRServer.Models;

public class DrawToMatchGame : AbstractGame
{
    public override void DrawCard(string username)
    {
        var playerDeck = PlayerDecks.FirstOrDefault(pd => pd.Username == username);
        if (playerDeck != null)
        {
            var random = new Random();
            var card = new NumberCard(((Colors)random.Next(0, 5)).ToString(), random.Next(0, 10));
            playerDeck.AddCard(card);
        }

        logger.LogInfo($"{username} drew a card. (DrawToMatchGame - player does not change)");
        // In DrawToMatch, the player does not change after drawing a card
    }
}