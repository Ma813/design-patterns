using SignalRServer.Models;

namespace SignalRServer.Helpers;

public class CardPlacementStrategy : ICardPlacementStrategy
{
    public bool CanPlaceCard(BaseCard topCard, BaseCard candidateCard)
    {
        return candidateCard.Color == topCard.Color
            || candidateCard.Name == topCard.Name;
    }
}

public interface ICardPlacementStrategy
{
    bool CanPlaceCard(BaseCard topCard, BaseCard candidateCard);
}
