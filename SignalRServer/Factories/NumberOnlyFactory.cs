using SignalRServer.Models;

namespace SignalRServer.Factories
{
    public class NumberOnlyFactory : IUnoCardFactory
    {
        private readonly Random random = new Random();

        public UnoCard GenerateCard()
        {
            string color = UnoCard.PossibleColors[random.Next(UnoCard.PossibleColors.Count)];
            int digit = random.Next(0, 10);
            return new UnoCard(color, digit);
        }
    }
}