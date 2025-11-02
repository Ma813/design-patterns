using SignalRServer.Card;

namespace SignalRServer.Models.CardPlacementStrategies;

public class UnoPlacementStrategy : ICardPlacementStrategy
{
    public bool CanPlaceCard(UnoCard topCard, UnoCard candidateCard)
    {
        return candidateCard.Color == topCard.Color
            || candidateCard.implementation.GetEffectDescription() == topCard.implementation.GetEffectDescription();
    }
}

