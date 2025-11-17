using Microsoft.AspNetCore.SignalR;

// Null object pattern for IHubCallerClients
public class NullClients : IHubCallerClients<IClientProxy>
{
    public IClientProxy All => new NullClientProxy();
    public IClientProxy AllExcept(IReadOnlyList<string> excludedConnectionIds) => new NullClientProxy();
    public IClientProxy Caller => new NullClientProxy();
    public IClientProxy Client(string connectionId) => new NullClientProxy();
    public IClientProxy Clients(IReadOnlyList<string> connectionIds) => new NullClientProxy();
    public IClientProxy Group(string groupName) => new NullClientProxy();
    public IClientProxy GroupExcept(string groupName, IReadOnlyList<string> excludedConnectionIds) => new NullClientProxy();
    public IClientProxy Groups(IReadOnlyList<string> groupNames) => new NullClientProxy();
    public IClientProxy Others => new NullClientProxy();
    public IClientProxy OthersInGroup(string groupName) => new NullClientProxy();
    public IClientProxy User(string userId) => new NullClientProxy();
    public IClientProxy Users(IReadOnlyList<string> userIds) => new NullClientProxy();
}

public class NullClientProxy : IClientProxy
{
    public Task SendCoreAsync(string method, object?[] args, CancellationToken cancellationToken = default)
    {
        // Do nothing
        return Task.CompletedTask;
    }
}