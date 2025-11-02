using SignalRServer.Models.Game;

namespace SignalRServer.Card;

public interface ICardDecorator
{
    void Play(AbstractGame game);
}