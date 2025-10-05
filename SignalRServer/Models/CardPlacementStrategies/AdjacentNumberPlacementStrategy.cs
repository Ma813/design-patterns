namespace SignalRServer.Models.CardPlacementStrategies
{
    public class AdjacentNumberPlacementStrategy : ICardPlacementStrategy
    {

        public bool CanPlaceCard(UnoCard topCard, UnoCard candidateCard)
        {
            
            return (candidateCard.Digit - 1) == topCard.Digit || (candidateCard.Digit + 1) == topCard.Digit;
        }
    }
}
