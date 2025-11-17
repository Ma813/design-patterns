using SignalRServer.Models.Game;

namespace SignalRServer.Card;

public class SkipTurnDecorator : CardDecorator
{
    public SkipTurnDecorator(UnoCard card) : base(card) { }

    public override string Name => $"{decoratedCard.Name} + Skip";

    public override void Play(AbstractGame game)
    {
        decoratedCard.Play(game);
        game.NextPlayer(Models.Action.place);
    }
}