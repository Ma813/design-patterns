using SignalRServer.Card;
using SignalRServer.Models.Game;

namespace SignalRServer.Models.Commands;

public abstract class Command
{
    protected AbstractGame game;
    protected PlayerDeck pd;
    protected PlayerDeck backup;
    protected UnoCard topCardBackup;

    public Command(AbstractGame game, PlayerDeck pd)
    {
        this.game = game;
        this.pd = pd;

        backup = new PlayerDeck("backup",game.Generator);
        backup.Cards.Clear();
        foreach (UnoCard uc in pd.Cards) backup.AddCard(uc);
    }

    public void SaveBackup()
    {
        backup.Cards.Clear();
        foreach (UnoCard uc in pd.Cards) backup.AddCard(uc);
        if (game.TopCard is NumberCard nc)
        {
            topCardBackup = new NumberCard(game.TopCard.Color, nc.Digit);
        }
        else if (game.TopCard is PowerCard pc)
        {
            topCardBackup = new PowerCard(game.TopCard.Color, pc.PowerType);
        }
        else if (game.TopCard is CardDecorator wc)
        {
            topCardBackup = wc;
        }
    }

    public void Undo()
    {
        pd.Cards.Clear();
        foreach (UnoCard uc in backup.Cards) pd.AddCard(uc);
        game.TopCard = topCardBackup;
    }

    public abstract override string ToString();

    public abstract bool Execute();
}