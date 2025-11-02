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

        backup = new PlayerDeck("backup");
        backup.Cards.Clear();
        foreach (UnoCard uc in pd.Cards) backup.AddCard(uc);
    }

    public void SaveBackup()
    {
        backup.Cards.Clear();
        foreach (UnoCard uc in pd.Cards) backup.AddCard(uc);
        topCardBackup = new UnoCard(game.TopCard.Color, game.TopCard.Digit);
    }

    public void Undo()
    {
        pd.Cards.Clear();
        foreach (UnoCard uc in backup.Cards) pd.AddCard(uc);
        game.TopCard = new UnoCard(topCardBackup.Color, topCardBackup.Digit);
    }

    public abstract override string ToString();

    public abstract bool Execute();
}