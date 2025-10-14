public class CommandHistory
{
    public Stack<Command> history = new Stack<Command>();

    public void Push(Command command)
    {
        history.Push(command);
    }

    public Command Pop()
    {
        return history.Count > 0 ? history.Pop() : null;
    }
}