namespace SignalRServer.Models;

public interface IBotClientPrototype
{
    string UserName { get; }
    IBotClientPrototype Clone(string newUserName);
    Task getNotified();
}