namespace SignalRServer.Models;

public class ReverseCard : BaseCard
{
    public ReverseCard(string color) : base(color, "Reverse") { }

    public override void Play()
    {
        Console.WriteLine($"{Color} Reverse card played. Direction of play is reversed!");
    }

    public override bool CanPlay(BaseCard card)
    {
        return Color == card.Color || card is ReverseCard;
    }
}