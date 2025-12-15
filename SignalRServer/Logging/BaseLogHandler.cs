namespace SignalRServer.Logging;

public abstract class BaseLogHandler : ILogHandler
{
    private ILogHandler _nextHandler;

    public void SetNext(ILogHandler handler)
    {
        _nextHandler = handler;
    }

    public virtual void Handle(LogLevelCustom level, string message, string source = "")
    {
        if (CanHandle(level))
        {
            ProcessLog(level, message, source);
        }

        _nextHandler?.Handle(level, message, source);
    }

    protected abstract bool CanHandle(LogLevelCustom level);
    protected abstract void ProcessLog(LogLevelCustom level, string message, string source);
}