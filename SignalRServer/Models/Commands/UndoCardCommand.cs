using SignalRServer.Models.Game;

namespace SignalRServer.Models.Commands;

public class UndoCardCommand : Command
{
    public UndoCardCommand(AbstractGame game, PlayerDeck pd) : base(game, pd) { }

    public override bool Execute()
    {
        pd.Undo();
        return false;
    }

    public override string ToString()
    {
        return string.Format("Undo previous action");
    }   
}