namespace SignalRServer.Models
{
    public class UnoCard
    {
        public static readonly List<string> PossibleColors = new List<string> { "red", "green", "blue", "yellow" };
        public static readonly List<string> ActionTypes = new List<string> { "Skip", "Reverse", "Draw Two" };

        public string Color { get; set; }
        public int? Digit { get; set; }
        public string? ActionType { get; set; }

        public bool IsAction => ActionType != null;

        public UnoCard(string color, int? digit = null, string? actionType = null)
        {
            Color = color;
            Digit = digit;
            ActionType = actionType;
        }

        public override string ToString()
        {
            return IsAction ? $"{Color} {ActionType}" : $"{Color} {Digit}";
        }

        public bool CanPlayOn(UnoCard topCard)
        {
            if (this.IsAction || topCard.IsAction)
                return this.Color == topCard.Color || this.ActionType == topCard.ActionType;

            return this.Color == topCard.Color || this.Digit == topCard.Digit;
        }

        public override bool Equals(object? obj)
        {
            if (obj is UnoCard other)
            {
                return this.Color == other.Color &&
                       this.Digit == other.Digit &&
                       this.ActionType == other.ActionType;
            }
            return false;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Color, Digit, ActionType);
        }
    }
}