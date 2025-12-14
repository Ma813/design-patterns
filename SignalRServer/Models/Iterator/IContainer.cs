namespace SignalRServer.Models.Iterator;

public interface IContainer<T>
{
    IIterator<T> CreateIterator();
    int Count { get; }
}