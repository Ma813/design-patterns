using SignalRServer.Models;

namespace SignalRServer.Builders;

public class GameDirector
{
    public AbstractGame Construct(
        IGameBuilder builder)
    {
        builder.CreateNewGame()
       .BuildPlayerCollections()
       .BuildCardFactory()
       .BuildPlacementStrategy()
       .BuildInitialState();

        return builder.GetResult();
    }
}
