using SignalRServer.Card;
using SignalRServer.Models.Game;

namespace SignalRServer.Models.Commands;

public class DrawCardCommand : Command
{
    UnoCard card;

    public DrawCardCommand(AbstractGame game, PlayerDeck pd) : base(game, pd)
    {
    }

    public override bool Execute()
    {
        SaveBackup();

        UnoCard newCard = UnoCard.GenerateCard();
        pd.Cards.Add(newCard);

        card = newCard;

        game.NotifyAll(Action.draw, card);

        return true;
    }

    public override string ToString()
    {
        return string.Format("Draw card - {0}", card.ToString());
    }
}