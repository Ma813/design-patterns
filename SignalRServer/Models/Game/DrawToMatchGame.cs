using SignalRServer.Helpers;

namespace SignalRServer.Models;

public class DrawToMatchGame : AbstractGame
{
    public DrawToMatchGame(CardGeneratingMode cardGeneratingMode, StrategyType placementStrategy) : base(cardGeneratingMode, placementStrategy)
    {
    }

    public override void DrawCard(string username)
    {
        var playerDeck = PlayerDecks.FirstOrDefault(pd => pd.Username == username);
        if (playerDeck != null)
        {
            var random = new Random();
            var card = cardFactory.GenerateCard();
            playerDeck.AddCard(card);
        }

        logger.LogInfo($"{username} drew a card. (DrawToMatchGame - player does not change)");
        // In DrawToMatch, the player does not change after drawing a card
    }
}