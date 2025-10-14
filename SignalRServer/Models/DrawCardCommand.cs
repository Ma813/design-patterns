using System.ComponentModel;
using SignalRServer.Models;

public class DrawCardCommand : Command
{
    UnoCard card;

    public DrawCardCommand(Game game, PlayerDeck pd) : base(game, pd)
    {
    }

    public override bool Execute()
    {
        SaveBackup();

        UnoCard newCard = UnoCard.GenerateCard();
        pd.Cards.Add(newCard);
        
        card = newCard;

        return true;
    }

    public override string ToString()
    {
        return string.Format("Draw card - {0}", card.ToString());
    }
}