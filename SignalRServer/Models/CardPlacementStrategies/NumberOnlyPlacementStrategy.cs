using SignalRServer.Card;

namespace SignalRServer.Models.CardPlacementStrategies;

public class NumberOnlyPlacementStrategy : ICardPlacementStrategy
{
    public bool CanPlaceCard(UnoCard topCard, UnoCard candidateCard)
    {
        if (topCard.implementation is NumberCardImplementation topNumberCard && candidateCard.implementation is NumberCardImplementation candidateNumberCard)
        {
            return candidateNumberCard.Number == topNumberCard.Number;
        }
        return true;
    }
}

