using SignalRServer.Card;
using SignalRServer.Models;
using SignalRServer.Models.Game;

public class TurnValidator : BaseCardPlayValidator
{
    public override string ValidateInternal(PlayerDeck pd, UnoCard card, AbstractGame game)
    {
        if(pd != game.PlayerDecks[game.CurrentPlayerIndex]) return "Not your turn";

        return "OK";
    }
}