using SignalRServer.Models;

namespace SignalRServer.Helpers;

public class FaceOnlyPlacementStrategy : ICardPlacementStrategy
{
    public bool CanPlaceCard(BaseCard topCard, BaseCard candidateCard)
    {
        return candidateCard.Name == topCard.Name;
    }
}
