using SignalRServer.Models.Game;

namespace SignalRServer.Card
{
    public class SkipEffectCard : UnoCard
    {
        public SkipEffectCard(string color) : base(color, new SkipEffectImplementation())
        {
        }

        public override void Play(AbstractGame game)
        {
            game.NextPlayer(Models.Action.place);
        }
    }

    public class SkipEffectImplementation : ICardImplementation
    {
        public string GetEffectDescription()
        {
            return "Skip";
        }

        public void ExecuteEffect(AbstractGame game)
        {
            game.NextPlayer(Models.Action.place);
        }
    }
}