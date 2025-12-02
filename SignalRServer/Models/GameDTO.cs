using SignalRServer.Card;
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
        if (game.PlayerDecks.Count == 0)
        {
            currentPlayer = "No one";
        }
        else
            currentPlayer = game.PlayerDecks[game.CurrentPlayerIndex].Username;
        direction = game.Direction;
        PlayerDeck = game.PlayerDecks.FirstOrDefault(pd => pd.Username == userName);
        CardCount = game.PlacedCardCount;
        if (PlayerDeck == null || PlayerDeck.history == null) return;
        foreach (Command cmd in PlayerDeck.history.history)
        {
            commandHistory.Add(cmd.ToString());
        }
    }

    public string ToConsoleString()
    {
        if (PlayerDeck == null)
        {
            return "\n"; // Game is probably over
        }
        string result = "Game State:\n";
        result += $"Current Player: {currentPlayer}\n";
        result += $"Direction: {(direction == 1 ? "Clockwise" : "Counter-Clockwise")}\n";
        result += "Player Amounts:\n";
        foreach (var player in PlayerAmounts)
        {
            result += $"- {player.Key} {(player.Key == PlayerDeck.Username ? "(You)" : "")}: {player.Value} cards\n";
        }

        result += $"Your Deck ({PlayerDeck.Username}):\n";

        for (int i = 0; i < PlayerDeck.Cards.Count; i++)
        {
            var card = PlayerDeck.Cards[i];
            result += $"{i}. {card}\n";
        }
        result += $"Top Card: {topCard}\n";
        result += $"Current Player: {currentPlayer} {(currentPlayer == PlayerDeck.Username ? "(You)" : "")}\n";

        return result;
    }
}
