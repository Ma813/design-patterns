using SignalRServer.Models;

namespace SignalRServer.Helpers;

public class ColorOnlyPlacementStrategy : ICardPlacementStrategy
{
    public bool CanPlaceCard(BaseCard topCard, BaseCard candidateCard)
    {
        return candidateCard.Color == topCard.Color;
    }
}
