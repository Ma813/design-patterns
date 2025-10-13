using SignalRServer.Models;

namespace SignalRServer.Helpers;

public class CardFactory : ICardFactory
{
    private readonly Random random = new();

    public BaseCard GenerateCard()
    {
        Colors color = (Colors)random.Next(0, Enum.GetValues(typeof(Colors)).Length);
        bool isAction = Convert.ToBoolean(random.Next(0, 2));

        if (isAction)
        {
            ActionTypes action = (ActionTypes)random.Next(0, Enum.GetValues(typeof(ActionTypes)).Length);
            return action switch
            {
                ActionTypes.Skip => new SkipCard(color.ToString()),
                ActionTypes.Reverse => new ReverseCard(color.ToString()),
                _ => throw new InvalidOperationException("Invalid action type"),
            };
        }
        else
        {
            int digit = random.Next(0, 10);
            return new NumberCard(color.ToString(), digit);
        }
    }
}

public interface ICardFactory
{
    BaseCard GenerateCard();
}
