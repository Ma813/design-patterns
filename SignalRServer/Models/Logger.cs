namespace SignalRServer.Models;

public sealed class Logger
{
    private static Logger? _instance = null;
    private readonly bool toConsole;

    private Logger(bool toConsole)
    {
        this.toConsole = toConsole;
    }

    private static readonly object _lock = new();

    public static Logger GetInstance()
    {
        if (_instance == null)
        {
            lock (_lock)
            {
                _instance ??= new Logger(true);
            }
        }

        return _instance;
    }

    public void LogInfo(string message)
    {
        if(toConsole) Console.WriteLine(message);
    }
} 