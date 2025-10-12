namespace SignalRServer.Models;

public class GameDto
{
    public PlayerDeck? PlayerDeck { get; set; } // The deck of the player requesting the game data
    public Dictionary<string, int> PlayersCardsCounts { get; set; }
    public BaseCard topCard { get; set; }
    public string currentPlayer { get; set; }
    public int direction { get; set; }

    public GameDto(Game game, string userName)
    {
        PlayersCardsCounts = [];
        foreach (PlayerDeck playerDeck in game.PlayerDecks)
        {
            PlayersCardsCounts[playerDeck.Username] = playerDeck.Count;
        }
        topCard = game.topCard;
        currentPlayer = game.PlayerDecks[game.currentPlayerIndex].Username;
        direction = game.direction;
        PlayerDeck = game.PlayerDecks.FirstOrDefault(pd => pd.Username == userName);
    }
}