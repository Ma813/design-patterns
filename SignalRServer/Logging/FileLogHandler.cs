using System.IO;

namespace SignalRServer.Logging;

public class FileLogHandler : BaseLogHandler
{
    private readonly string _logFilePath;

    public FileLogHandler(string logFilePath = "game.log")
    {
        _logFilePath = logFilePath;
    }

    protected override bool CanHandle(LogLevelCustom level)
    {
        return level >= LogLevelCustom.Warning;
    }

    protected override void ProcessLog(LogLevelCustom level, string message, string source)
    {
        var timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
        var levelStr = level.ToString().ToUpper();
        var logEntry = $"[{timestamp}] [{levelStr}] {source}: {message}{Environment.NewLine}";
        
        File.AppendAllText(_logFilePath, logEntry);
    }
}