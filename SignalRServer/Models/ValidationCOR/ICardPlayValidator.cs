using SignalRServer.Card;
using SignalRServer.Models;
using SignalRServer.Models.Game;

public interface ICardPlayValidator
{
    public void SetNext(ICardPlayValidator validator);
    public string Validate(PlayerDeck pd, UnoCard c, AbstractGame game);
}