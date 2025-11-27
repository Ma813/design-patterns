namespace SignalRServer.Visitors;

public interface IDeckVisitor
{
    void Visit(SignalRServer.Models.PlayerDeck deck);
}