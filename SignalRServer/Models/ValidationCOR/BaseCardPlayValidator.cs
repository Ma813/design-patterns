using SignalRServer.Card;
using SignalRServer.Models;
using SignalRServer.Models.Game;

public abstract class BaseCardPlayValidator : ICardPlayValidator
{
    protected ICardPlayValidator _next = null;

    public void SetNext(ICardPlayValidator validator)
    {
        _next = validator;
    }

    public string Validate(PlayerDeck pd, UnoCard card, AbstractGame game)
    {
        var result = ValidateInternal(pd, card, game);

        if(result != "OK" || _next == null) 
            return result;

        return _next.Validate(pd, card, game);
    }

    public abstract string ValidateInternal(PlayerDeck pd, UnoCard card, AbstractGame game);
}