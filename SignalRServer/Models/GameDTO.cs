using SignalRServer.Models.Commands;
using SignalRServer.Models.Game;

namespace SignalRServer.Models;

public class GameForSending
{
    public Dictionary<string, int> PlayerAmounts { get; set; }
    public UnoCard topCard { get; set; }
    public string currentPlayer { get; set; }
    public int direction { get; set; }
    public PlayerDeck? PlayerDeck { get; set; } // The deck of the player requesting the game state
    public Dictionary<string, int> CardCount { get; set; }
    public List<string> commandHistory { get; set; } = [];

    public GameForSending(AbstractGame game, string userName)
    {
        PlayerAmounts = [];
        foreach (PlayerDeck pDeck in game.PlayerDecks)
        {
            PlayerAmounts[pDeck.Username] = pDeck.Count;
        }
        topCard = game.TopCard;
        currentPlayer = game.PlayerDecks[game.CurrentPlayerIndex].Username;
        direction = game.Direction;
        PlayerDeck = game.PlayerDecks.FirstOrDefault(pd => pd.Username == userName);
        CardCount = game.PlacedCardCount;
        foreach (Command cmd in PlayerDeck.history.history)
        {
            commandHistory.Add(cmd.ToString());
        }
    }
}
