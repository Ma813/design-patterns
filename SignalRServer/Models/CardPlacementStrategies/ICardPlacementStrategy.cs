using SignalRServer.Card;

namespace SignalRServer.Models.CardPlacementStrategies;

public interface ICardPlacementStrategy
{
    bool CanPlaceCard(UnoCard topCard, UnoCard candidateCard);
}
