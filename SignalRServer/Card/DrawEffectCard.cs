using SignalRServer.Models.Game;

namespace SignalRServer.Card
{
    public class DrawEffectCard : UnoCard
    {
        public DrawEffectCard(string color) : base(color, new DrawEffectImplementation())
        {
        }

        public override void Play(AbstractGame game)
        {
            game.NextDrawCard();
        }
    }

    public class DrawEffectImplementation : ICardImplementation
    {
        public string GetEffectDescription()
        {
            return "Draw+1";
        }

        public void ExecuteEffect(AbstractGame game)
        {
            game.NextDrawCard();
        }
    }
}