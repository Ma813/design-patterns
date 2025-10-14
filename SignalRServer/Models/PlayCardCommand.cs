using SignalRServer.Models;

public class PlayCardCommand : Command
{
    protected UnoCard card;

    public PlayCardCommand(Game game, PlayerDeck pd, UnoCard card) : base(game, pd)
    {
        this.card = card;
    } 

    public override bool Execute()
    {
        SaveBackup();

        if (game.PlayerDecks[game.currentPlayerIndex].Username != pd.Username)
        {
            return false; // Not this player's turn
        }

        if (!card.CanPlayOn(game.topCard))
        {
            return false; // Invalid move
        }

        game.topCard = card;
        pd.Cards.Remove(card);

        return true;
    }   
}