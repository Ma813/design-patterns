using SignalRServer.Models;

public abstract class ISubject
{
    protected List<IObserver> observers = new List<IObserver>();
	
	public void Add(IObserver o)
	{
		observers.Add(o);
		o.SetSubject(this);
	}
	
	public void Remove(IObserver o)
	{
		observers.Remove(o);
	}
	
	public void Broadcast(Action a, UnoCard c, PlayerDeck pd)
	{
		foreach (var o in observers) o.Update(a, c, pd);
	}
}