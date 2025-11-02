using SignalRServer.Card;

namespace SignalRServer.Models.CardPlacementStrategies;

public class AdjacentNumberPlacementStrategy : ICardPlacementStrategy
{
    public bool CanPlaceCard(UnoCard topCard, UnoCard candidateCard)
    {
        if (topCard.implementation is NumberCardImplementation topNumberCard && candidateCard.implementation is NumberCardImplementation candidateNumberCard)
        {
            return (candidateNumberCard.Number - 1) == topNumberCard.Number || (candidateNumberCard.Number + 1) == topNumberCard.Number;
        }
        return candidateCard.Color == topCard.Color;
    }
}
