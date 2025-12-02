using SignalRServer.Card;
using SignalRServer.Models;
using SignalRServer.Models.Game;

public class PlacementStrategyValidator : BaseCardPlayValidator
{
    public override string ValidateInternal(PlayerDeck pd, UnoCard card, AbstractGame game)
    {
        if(!card.CanPlayOn(game.TopCard, game.CardPlacementStrategy)) return "Card cannot be played on top of current top card";

        return "OK";
    }
}