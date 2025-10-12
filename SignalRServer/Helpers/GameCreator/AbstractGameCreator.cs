using SignalRServer.Models;

namespace SignalRServer.Helpers;

public abstract class AbstractGameCreator
{
    public abstract AbstractGame CreateGame(string gameMode = "Classic");
}