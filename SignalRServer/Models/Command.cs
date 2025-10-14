using SignalRServer.Models;

public abstract class Command
{
    protected Game game;
    protected PlayerDeck pd;
    protected PlayerDeck backup;
    protected UnoCard topCardBackup;

    public Command(Game game, PlayerDeck pd)
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
        topCardBackup = new UnoCard(game.topCard.Color, game.topCard.Digit);
    }

    public void Undo()
    {
        pd.Cards.Clear();
        foreach (UnoCard uc in backup.Cards) pd.AddCard(uc);
        game.topCard = new UnoCard(topCardBackup.Color, topCardBackup.Digit);
    }

    public abstract override string ToString();

    public abstract bool Execute();
}