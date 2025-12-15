namespace SignalRServer.Logging;

public class DatabaseLogHandler : BaseLogHandler
{
    protected override bool CanHandle(LogLevelCustom level)
    {
        return level >= LogLevelCustom.Error;
    }

    protected override void ProcessLog(LogLevelCustom level, string message, string source)
    {
        System.Diagnostics.Debug.WriteLine($"DB_LOG: [{level}] {source} - {message}");
    }
}