using SignalRServer.Card;

namespace SignalRServer.Models;

public abstract class ISubject
{
    protected List<IObserver> observers = [];

	public void Add(IObserver o)
	{
		observers.Add(o);
		o.SetSubject(this);
	}

	public void Remove(IObserver o)
	{
		observers.Remove(o);
	}

	public void NotifyAll(Action a, UnoCard c)
	{
		foreach (var o in observers) o.Update(a, c);
	}
}