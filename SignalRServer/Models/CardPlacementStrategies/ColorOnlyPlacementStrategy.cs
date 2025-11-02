namespace SignalRServer.Models.CardPlacementStrategies;

public class ColorOnlyPlacementStrategy : ICardPlacementStrategy
{
    public bool CanPlaceCard(UnoCard topCard, UnoCard candidateCard)
    {
        return candidateCard.Color == topCard.Color;
    }
}
