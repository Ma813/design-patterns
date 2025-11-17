namespace SignalRServer.Models;

public interface IBotClientPrototype 
{ 
    string UserName { get; } 
    int DifficultyLevel { get; } 
    string Strategy { get; } 
    IBotClientPrototype Clone(string newUserName); 
    Task getNotified(); 
} 