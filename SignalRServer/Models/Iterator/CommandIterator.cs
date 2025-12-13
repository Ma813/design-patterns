using SignalRServer.Models.Commands;

namespace SignalRServer.Models.Iterator;

public class CommandIterator : IIterator<Command>
{
    private readonly List<Command> _commands;
    private int _currentIndex;

    public CommandIterator(Stack<Command> commandHistory, bool reverse, Func<Command, bool> predicate)
    {
        _commands = commandHistory
            .Where(predicate)
            .ToList();

        if(reverse) _commands.Reverse();
        
        Reset();
    }

    public bool IsDone()
    {
        return _currentIndex >= _commands.Count;
    }

    public bool HasNext()
    {
        return _currentIndex < _commands.Count;
    }

    public void Next()
    {
        _currentIndex++;
    }

    public void Reset()
    {
        _currentIndex = 0;
    }

    public Command? Current()
    {
        if (_currentIndex >= _commands.Count || _currentIndex < 0)
        {
            return null;
        }

        return _commands[_currentIndex];
    }
}