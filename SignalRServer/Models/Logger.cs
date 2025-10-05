namespace SignalRServer.Models;

public sealed class Logger
{
    private static Logger _instance = null;
    private bool toConsole;

    private Logger(bool toConsole)
    {
        this.toConsole = toConsole;
    }

    private static readonly object _lock = new object();

    public static Logger GetInstance()
    {
        if (_instance == null)
        {
            lock (_lock)
            {
                if (_instance == null) _instance = new Logger(true);
            }
        }

        return _instance;
    }

    public void LogInfo(string message)
    {
        if(toConsole) Console.WriteLine(message);
    }
}