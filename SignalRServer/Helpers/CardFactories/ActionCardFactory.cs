using SignalRServer.Models;

namespace SignalRServer.Helpers;

public class ActionCardFactory : ICardFactory
{
    private readonly Random random = new();

    public BaseCard GenerateCard()
    {
        Colors color = (Colors)random.Next(0, 5);
        ActionTypes action = (ActionTypes)random.Next(0, 3);
        return action switch
        {
            ActionTypes.Skip => new SkipCard(color.ToString()),
            ActionTypes.Reverse => new ReverseCard(color.ToString()),
            _ => throw new InvalidOperationException("Invalid action type"),
        };
    }
}