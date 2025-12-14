namespace SignalRServer.Models.Chat;

public interface IChatMediator
{
    void Register(IChatColleague colleague);
    void Unregister(string username);
    Task SendMessage(string senderUsername, string text, string roomName);
    void Mute(string muter, string target);
    void Unmute(string muter, string target);
}