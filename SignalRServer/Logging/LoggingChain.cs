namespace SignalRServer.Logging;

public class LoggingChain
{
    private readonly ILogHandler _firstHandler;

    public LoggingChain()
    {
        // Create the chain: Console -> File -> Database -> Email
        var consoleHandler = new ConsoleLogHandler();
        var fileHandler = new FileLogHandler();
        var dbHandler = new DatabaseLogHandler();

        // Set up the chain
        consoleHandler.SetNext(fileHandler);
        fileHandler.SetNext(dbHandler);

        _firstHandler = consoleHandler;
    }

    public void Log(LogLevelCustom level, string message, string source = "System")
    {
        _firstHandler.Handle(level, message, source);
    }
}