namespace SignalRServer.Models.Game;

// Abstract factory
public abstract class AbstractGameCreator
{
    public abstract AbstractGame CreateGame(string gameMode = "Classic", string roomName = "DefaultRoom");
}
