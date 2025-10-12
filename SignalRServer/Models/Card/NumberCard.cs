namespace SignalRServer.Models;

public class NumberCard : BaseCard
{
    public int Number { get; }

    public NumberCard(string color, int number) : base(color, number.ToString())
    {
        Name = number.ToString();
        Number = number;
    }

    public override void Play(AbstractGame game)
    {
        Console.WriteLine($"{Color} {Number} card played.");
    }

    public override bool CanPlay(BaseCard card)
    {
        return Color == card.Color || Number.ToString() == card.Name;
    }
}