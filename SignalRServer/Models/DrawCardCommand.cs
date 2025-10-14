using SignalRServer.Models;

public class DrawCardCommand : Command
{
    public DrawCardCommand(Game game, PlayerDeck pd) : base(game, pd)
    {
    } 

    public override bool Execute()
    {
        SaveBackup();

        UnoCard newCard = UnoCard.GenerateCard();
        pd.Cards.Add(newCard);

        return true;
    }   
}