using SignalRServer.Card;
using SignalRServer.Models;
using SignalRServer.Models.Game;

public class NullValidator : BaseCardPlayValidator
{
    public override string ValidateInternal(PlayerDeck pd, UnoCard card, AbstractGame game)
    {
        if(pd == null) return "Player not found";

        return "OK";
    }
}