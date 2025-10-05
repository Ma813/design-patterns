namespace SignalRServer.Models.CardPlacementStrategies
{
    public class NumberOnlyPlacementStrategy : ICardPlacementStrategy
    {
        public bool CanPlaceCard(UnoCard topCard, UnoCard candidateCard)
        {
            return  candidateCard.Digit == topCard.Digit;
        }
    }
}
