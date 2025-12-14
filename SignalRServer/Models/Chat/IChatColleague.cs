namespace SignalRServer.Models.Chat;

public interface IChatColleague
{
    string Username { get; }
    string ConnectionId { get; }
    Task Receive(Message message);
    void SetMediator(IChatMediator mediator);
}