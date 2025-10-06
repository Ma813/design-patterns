using SignalRServer.Models;

namespace SignalRServer.Factories
{
    public class SameNumberFactory : IUnoCardFactory
    {
        private readonly int fixedDigit;
        private readonly Random random = new Random();

        public SameNumberFactory(int digit)
        {
            fixedDigit = digit;
        }

        public UnoCard GenerateCard()
        {
            string color = UnoCard.PossibleColors[random.Next(UnoCard.PossibleColors.Count)];
            return new UnoCard(color, fixedDigit);
        }
    }
}