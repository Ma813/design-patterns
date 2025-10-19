using System.Security.Cryptography.X509Certificates;
using SignalRServer.Models;

public class CardCountUpdater : IObserver
{
    Game subject = null!;

    private Dictionary<string, int> cardCount;

    public CardCountUpdater()
    {
        cardCount = new Dictionary<string, int>();
    }

    public void Update(Action a, UnoCard c)
    {
        if (a == Action.draw) return;

        if (cardCount.ContainsKey(c.Color)) cardCount[c.Color]++;
        else cardCount[c.Color] = 1;

        if (cardCount.ContainsKey(c.Digit.ToString())) cardCount[c.Digit.ToString()]++;
        else cardCount[c.Digit.ToString()] = 1;

        subject.SetCardCount(cardCount);
    }
    public ISubject GetSubject()
    {
        return subject;
    }
    public void SetSubject(ISubject subject)
    {
        this.subject = (Game)subject;
    }
}