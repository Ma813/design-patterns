namespace SignalRServer.Models;

public class SkipCard : BaseCard
{
    public SkipCard(string color) : base(color, "Skip") { }

    public override void Play(AbstractGame game)
    {
        game.NextPlayer();
    }

    public override bool CanPlay(BaseCard card)
    {
        return Color == card.Color || card is SkipCard;
    }
}