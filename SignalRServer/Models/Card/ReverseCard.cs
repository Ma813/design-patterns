namespace SignalRServer.Models;

public class ReverseCard : BaseCard
{
    public ReverseCard(string color) : base(color, "Reverse") { }

    public override void Play(AbstractGame game)
    {
        game.Direction *= -1; // Flip direction of play
    }
}