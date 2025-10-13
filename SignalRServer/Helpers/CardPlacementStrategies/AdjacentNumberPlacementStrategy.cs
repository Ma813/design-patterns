using SignalRServer.Models;

namespace SignalRServer.Helpers;

public class AdjacentNumberPlacementStrategy : ICardPlacementStrategy
{

    public bool CanPlaceCard(BaseCard topCard, BaseCard candidateCard)
    {
        if (topCard.Color == candidateCard.Color) return true;

        if (topCard is NumberCard && candidateCard is NumberCard)
        {
            return int.Parse(candidateCard.Name) - 1 == int.Parse(topCard.Name)
                || int.Parse(candidateCard.Name) + 1 == int.Parse(topCard.Name);
        }
        else
        {
            return candidateCard.Name == topCard.Name;
        }
    }
}