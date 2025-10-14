using SignalRServer.Models;

public class UndoCardCommand : Command
{
    public UndoCardCommand(Game game, PlayerDeck pd) : base(game, pd) { } 

    public override bool Execute()
    {
        pd.Undo();
        return false;
    }   
}