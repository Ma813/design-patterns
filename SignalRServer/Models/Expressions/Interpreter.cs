namespace SignalRServer.Expressions;

public class Interpreter
{
    public static IExpression Parse(string command)
    {

        //TODO move all non player invoked results to facede so it would work for mixed lobbies
        string firstWord = command.Split(' ', 2, StringSplitOptions.RemoveEmptyEntries)[0].ToLower();
        return firstWord.ToLower() switch
        {
            "help" => new HelpExpression(),
            "join" => new JoinExpression(),
            "start" => new StartExpression(),
            "play" => new PlayExpression(),
            "draw" => new DrawExpression(),
            _ => new UnknownExpression(),
        };
    }
}