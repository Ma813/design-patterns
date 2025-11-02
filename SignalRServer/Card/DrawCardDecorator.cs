using SignalRServer.Models.Game;

namespace SignalRServer.Card;

public class DrawCardDecorator : CardDecorator
{

    public DrawCardDecorator(UnoCard card) : base(card)
    {
    }

    public override string Name => $"{decoratedCard.Name} + Draw";

    public override void Play(AbstractGame game)
    {
        decoratedCard.Play(game);
        game.NextDrawCard();
    }
}