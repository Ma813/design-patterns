using SignalRServer.Models.Game;

namespace SignalRServer.Card
{
    public class CompositeCardImplementation : ICardImplementation
    {
        public string GetEffectDescription()
        {
            return "Multi-Effect";
        }

        public void ExecuteEffect(AbstractGame game)
        {
            // Composite cards handle execution through their Play() method
            // This Execute method is not used for composite cards
        }
    }
}