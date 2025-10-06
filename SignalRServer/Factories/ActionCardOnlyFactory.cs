using SignalRServer.Models;

namespace SignalRServer.Factories
{
    public class ActionCardFactory : IUnoCardFactory
    {
        private readonly Random random = new Random();

        public UnoCard GenerateCard()
        {
            string color = UnoCard.PossibleColors[random.Next(UnoCard.PossibleColors.Count)];
            string action = UnoCard.ActionTypes[random.Next(UnoCard.ActionTypes.Count)];
            return new UnoCard(color, null, action);
        }
    }
}