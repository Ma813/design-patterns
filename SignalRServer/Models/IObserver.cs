namespace SignalRServer.Models;

public enum Action
{
    draw,
    place
}

public interface IObserver
{
    void Update(Action a, UnoCard c);
    public ISubject GetSubject();
    public void SetSubject(ISubject subject);
}