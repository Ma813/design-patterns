namespace SignalRServer.Models;

public class GameDto
{
    public PlayerDeck? PlayerDeck { get; set; } // The deck of the player requesting the game data
    public Dictionary<string, int> PlayersCardsCounts { get; set; }
    public BaseCard TopCard { get; set; }
    public string CurrentPlayer { get; set; }
    public int Direction { get; set; }

    public GameDto(AbstractGame game, string userName)
    {
        PlayersCardsCounts = [];
        foreach (PlayerDeck playerDeck in game.PlayerDecks)
        {
            PlayersCardsCounts[playerDeck.Username] = playerDeck.Count;
        }
        TopCard = game.TopCard;
        CurrentPlayer = game.PlayerDecks[game.CurrentPlayerIndex].Username;
        Direction = game.Direction;
        PlayerDeck = game.PlayerDecks.FirstOrDefault(pd => pd.Username == userName);
    }
}