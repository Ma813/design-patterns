using System.ComponentModel;
using SignalRServer.Models.Iterator;

namespace SignalRServer.Models.Commands;

public class CommandHistory : IContainer<Command>
{
    public Stack<Command> history = new Stack<Command>();

    public int Count => history.Count;

    public void Push(Command command)
    {
        history.Push(command);
    }

    public Command? Pop()
    {
        return history.Count > 0 ? history.Pop() : null;
    }

    public IIterator<Command> CreateIterator(bool reverse, Func<Command, bool> predicate)
    {
        return new CommandIterator(history, reverse, predicate);
    }

    public IIterator<Command> CreateIterator()
    {
        return new CommandIterator(history, false, null);
    }
}