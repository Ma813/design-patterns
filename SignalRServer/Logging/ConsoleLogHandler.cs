namespace SignalRServer.Logging;

public class ConsoleLogHandler : BaseLogHandler
{
    protected override bool CanHandle(LogLevelCustom level)
    {
        return level >= LogLevelCustom.Info;
    }

    protected override void ProcessLog(LogLevelCustom level, string message, string source)
    {
        var timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
        var levelStr = level.ToString().ToUpper();
        Console.WriteLine($"[{timestamp}] [{levelStr}] {source}: {message}");
    }
}