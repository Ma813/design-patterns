using SignalRServer.Models.CardPlacementStrategies;

namespace SignalRServer.Card;

public abstract class CardDecorator : UnoCard
{
    protected UnoCard decoratedCard;

    protected CardDecorator(UnoCard card)
        : base(card.Color, card.implementation)
    {
        decoratedCard = card;
        Digit = card.Digit;
    }

    public override bool CanPlayOn(UnoCard topCard, ICardPlacementStrategy cardPlacementStrategy)
    {
        return decoratedCard.CanPlayOn(topCard, cardPlacementStrategy);
    }
}