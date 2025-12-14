using SignalRServer.Models.CardPlacementStrategies;
using SignalRServer.Models.Game;

namespace SignalRServer.Card
{
    public interface ICardComponent
    {
        string Color { get; }
        string Name { get; }
        void Play(AbstractGame game);
        bool CanPlayOn(UnoCard topCard, ICardPlacementStrategy strategy);
        void Add(ICardComponent component);
        void Remove(ICardComponent component);
        ICardComponent GetChild(int index);
        int GetChildCount();
    }
}