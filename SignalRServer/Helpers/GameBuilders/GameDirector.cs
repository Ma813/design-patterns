using SignalRServer.Models;
using System.Collections.Generic;

namespace SignalRServer.Builders;

public class GameDirector
{
    public AbstractGame Construct(IGameBuilder builder, Dictionary<string, string> players)
    {
        builder.BuildGame();
        builder.SetupPlayers(players);
        builder.SetupInitialState();
        return builder.GetResult();
    }
}
