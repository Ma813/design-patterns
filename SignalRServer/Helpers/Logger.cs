namespace SignalRServer.Helpers;

public sealed class Logger
{
    private static Logger? _instance = null;

    private Logger() {}

    private static readonly object _lock = new();

    public static Logger GetInstance()
    {
        if (_instance == null)
        {
            lock (_lock)
            {
                _instance ??= new Logger();
            }
        }

        return _instance;
    }

    public void LogInfo(string message)
    {
        Console.WriteLine(message);
    }
} 