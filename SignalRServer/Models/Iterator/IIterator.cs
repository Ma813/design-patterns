namespace SignalRServer.Models.Iterator;

public interface IIterator<T>
{
    void Reset();
    void Next();
    bool IsDone();
    T Current();
    bool HasNext();
}