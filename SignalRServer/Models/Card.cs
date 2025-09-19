namespace SignalRServer.Models
{
    public class UnoCard
    {
        public static readonly List<string> PossibleColors = new List<string> { "red", "green", "blue", "yellow" };
        public string Color { get; set; }
        public int Digit { get; set; }

        public UnoCard(string color, int digit)
        {
            Color = color;
            Digit = digit;
        }
        
        public static UnoCard GenerateCard()
        {
            var random = new Random();
            string color = PossibleColors[random.Next(PossibleColors.Count)];
            int digit = random.Next(0, 10); // Uno digits are 0-9
            return new UnoCard(color, digit);
        }
    }
}