using System.Runtime.InteropServices;
using SignalRServer.Model.Chat.Colleagues;

namespace SignalRServer.Models.Chat;

public class ChatMediator : IChatMediator
{
    private static readonly Logger logger = Logger.GetInstance();

    private readonly Dictionary<string, IChatColleague> _colleagues = new();
    private readonly Dictionary<string, HashSet<string>> _mutedBy = new();
    private readonly string _roomName;

    public ChatMediator(string roomName)
    {
        _roomName = roomName;
    }

    public void Register(IChatColleague colleague)
    {
        _colleagues[colleague.Username] = colleague;
        _mutedBy[colleague.Username] = new HashSet<string>();
        colleague.SetMediator(this);

        logger.LogInfo($"[ChatMediator] Registered colleague: {colleague.Username} ({colleague.GetType().Name})");
    }

    public void Unregister(string username)
    {
        _colleagues.Remove(username);
        _mutedBy.Remove(username);

        foreach(var mutedSet in _mutedBy.Values)
        {
            mutedSet.Remove(username);
        }

        logger.LogInfo($"[ChatMediator] Unregistered colleague: {username}");
    }

    public async Task SendMessage(string senderUsername, string text, string roomName)
    {
        if(roomName != _roomName) return;

        var message = new Message(senderUsername, text);

        logger.LogInfo($"[ChatMediator] Mediating message from {senderUsername} to {_colleagues.Count} colleagues");

        foreach(var (username, colleague) in _colleagues)
        {
            // if(username == senderUsername) return;

            if(_mutedBy.ContainsKey(senderUsername) && _mutedBy[senderUsername].Contains(username))
            {
                logger.LogInfo($"[ChatMediator] Message blocked: {senderUsername} has muted {username}");
                continue;
            }

            //System.Console.WriteLine("{0} sending to {1}", senderUsername, username);
            await colleague.Receive(message);
        }
    }

    public void Mute(string muter, string target)
    {
        if(!_mutedBy.ContainsKey(muter))
            _mutedBy[muter] = new HashSet<string>();

        if (_mutedBy[muter].Contains(target)){
            _mutedBy[muter].Remove(target);
        }
        else {
            _mutedBy[muter].Add(target);
            logger.LogInfo($"[ChatMediator] {muter} muted {target}");
        }
    }

    public void Unmute(string muter, string target)
    {
        if (_mutedBy.ContainsKey(muter))
        {
            _mutedBy[muter].Remove(target);
            logger.LogInfo($"[ChatMediator] {muter} unmuted {target}");
        }
    }

    public List<string> GetColleagueTypes()
    {
        return _colleagues.Values.Select(c => c.GetType().Name).Distinct().ToList();
    }
}