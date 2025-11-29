using SignalRServer.Card;
using SignalRServer.Models;
using SignalRServer.Models.Game;

public class CardOwnershipValidator : BaseCardPlayValidator
{
    public override string ValidateInternal(PlayerDeck pd, UnoCard card, AbstractGame game)
    {
        if(!pd.Cards.Contains(card)) return "Player does not have that card";

        return "OK";
    }
}