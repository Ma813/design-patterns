namespace SignalRServer.Logging;

public interface ILogHandler
{
    void SetNext(ILogHandler handler);
    void Handle(LogLevelCustom level, string message, string source = "");
}

public enum LogLevelCustom
{
    Debug = 0,
    Info = 1,
    Warning = 2,
    Error = 3,
    Critical = 4
}