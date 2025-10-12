using SignalRServer.Models;

public class CardCountUpdater : IObserver
{
    ISubject subject = null!;

    public void Update(Action a, UnoCard c, PlayerDeck pd)
    {
        Console.WriteLine("Player {0} {1} card with {2} and {3} number", a, pd.Username, c.Color, c.Digit);
        if(a == Action.place && c.Color == "red") pd.redCardCount++;
    }
    public ISubject GetSubject()
    {
        return subject;
    }
    public void SetSubject(ISubject subject)
    {
        this.subject = subject;
    }
}