using SignalRServer.Models.CardPlacementStrategies;
using SignalRServer.Models.Game;

namespace SignalRServer.Models.Commands;

public class PlayCardCommand : Command
{
    protected UnoCard card;
    protected ICardPlacementStrategy placementStrategy;

    public PlayCardCommand(AbstractGame game, PlayerDeck pd, UnoCard card, ICardPlacementStrategy placementStrategy) : base(game, pd)
    {
        this.card = card;
        this.placementStrategy = placementStrategy;
    }

    public override bool Execute()
    {
        SaveBackup();

        if (game.PlayerDecks[game.CurrentPlayerIndex].Username != pd.Username)
        {
            return false; // Not this player's turn
        }

        if (!card.CanPlayOn(game.TopCard, placementStrategy))
        {
            return false; // Invalid move
        }

        game.TopCard = card;
        pd.Cards.Remove(card);
        game.NotifyAll(Action.place, card);

        return true;
    }

    public override string ToString()
    {
        return string.Format("Played card - {0}", card.ToString());
    }   
}