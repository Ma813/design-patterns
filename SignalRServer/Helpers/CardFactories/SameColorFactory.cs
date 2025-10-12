using SignalRServer.Models;

namespace SignalRServer.Helpers;

public class SameColorFactory : ICardFactory
{
    private readonly string fixedColor;
    private readonly Random random = new();

    public SameColorFactory(string color)
    {
        fixedColor = color;
    }

    public BaseCard GenerateCard()
    {
        bool isAction = Convert.ToBoolean(random.Next(0, 2));

        if (isAction)
        {
            ActionTypes action = (ActionTypes)random.Next(0, 3);
            return action switch
            {
                ActionTypes.Skip => new SkipCard(fixedColor.ToString()),
                ActionTypes.Reverse => new ReverseCard(fixedColor.ToString()),
                _ => throw new InvalidOperationException("Invalid action type"),
            };
        }
        else
        {
            int digit = random.Next(0, 10);
            return new NumberCard(fixedColor.ToString(), digit);
        }
    }
}