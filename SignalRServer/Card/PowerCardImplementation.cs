using SignalRServer.Models.Game;
using SignalRServer.Models.CardPlacementStrategies;
namespace SignalRServer.Card;

public class PowerCardImplementation : ICardImplementation
{
    public string Color { get; }
    public string PowerType { get; }

    public PowerCardImplementation(string color, string powerType)
    {
        Color = color;
        PowerType = powerType;
    }

    public void ExecuteEffect(AbstractGame game)
    {
        Console.WriteLine($"Executing Power Card Effect: {PowerType}");
        switch (PowerType)
        {
            case "Skip":
                game.NextPlayer(Models.Action.place);
                break;
            case "Draw":
                game.NextDrawCard();
                break;
            case "RestoreHand":
                RestorePlayerHand(game);
                break;
        }
    }
    private void RestorePlayerHand(AbstractGame game)
    {
        if (game.PlayerDecks.Count == 0)
            return;

        var player = game.PlayerDecks[game.CurrentPlayerIndex];
        bool restored = player.RestoreLastTurnState();
        if (!restored)
        {
            return;
        }
        var restoreCard = player.Cards
            .OfType<PowerCard>()
            .FirstOrDefault(c => c.PowerType == "RestoreHand");

        if (restoreCard != null)
        {
            player.RemoveCard(restoreCard);
        }
    }

    public string GetEffectDescription()
    {
        return PowerType;
    }
}