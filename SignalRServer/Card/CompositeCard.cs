using SignalRServer.Models.CardPlacementStrategies;
using SignalRServer.Models.Game;

namespace SignalRServer.Card
{
    public class CompositeCard : UnoCard
    {
        private List<ICardComponent> _components = new List<ICardComponent>();

        public CompositeCard(string color) : base(color, new CompositeCardImplementation())
        {
        }

        public override void Add(ICardComponent component)
        {
            _components.Add(component);
        }

        public override void Remove(ICardComponent component)
        {
            _components.Remove(component);
        }

        public override ICardComponent GetChild(int index)
        {
            if (index >= 0 && index < _components.Count)
                return _components[index];
            throw new ArgumentOutOfRangeException(nameof(index));
        }

        public override int GetChildCount()
        {
            return _components.Count;
        }

        public override void Play(AbstractGame game)
        {
            // Execute all child card effects in sequence
            foreach (var component in _components)
            {
                component.Play(game);
            }
        }

        public override string Name => $"Combo({string.Join("+", _components.Select(c => c.Name))})";

        public override bool CanPlayOn(UnoCard topCard, ICardPlacementStrategy cardPlacementStrategy)
        {
            // A composite card can be played if any of its components can be played
            return _components.Any(component => 
                component.CanPlayOn(topCard, cardPlacementStrategy));
        }
    }
}