using SignalRServer.Models;

namespace SignalRServer.Factories
{
    public class SameColorFactory : IUnoCardFactory
    {
        private readonly string fixedColor;
        private readonly Random random = new Random();

        public SameColorFactory(string color)
        {
            fixedColor = color;
        }

        public UnoCard GenerateCard()
        {
            int digit = random.Next(0, 10);
            return new UnoCard(fixedColor, digit);
        }
    }
}