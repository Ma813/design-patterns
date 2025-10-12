namespace SignalRServer.Models;

public class SkipCard : BaseCard
{
    public SkipCard(string color) : base(color, "Skip") { }

    public override void Play()
    {
        Console.WriteLine($"{Color} Skip card played. Next player is skipped!");
    }

    public override bool CanPlay(BaseCard card)
    {
        return Color == card.Color || card is SkipCard;
    }
}