using SignalRServer.Models.Game;

namespace SignalRServer.Card;

public interface ICardImplementation
{
    void ExecuteEffect(AbstractGame game);
    string GetEffectDescription();
}