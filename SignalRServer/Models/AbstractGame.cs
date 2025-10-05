using System.Collections.Generic;
using System.Linq;
using SignalRServer.Models.CardPlacementStrategies;
namespace SignalRServer.Models
{
    public abstract class AbstractGame
    {
        public List<PlayerDeck> PlayerDecks { get; set; }
        public Dictionary<string, string> Players { get; set; } // {connectionId: username}
        public UnoCard TopCard { get; set; }
        public bool IsStarted { get; set; }
        public int CurrentPlayerIndex { get; set; }
        public int Direction { get; set; } // 1 for clockwise, -1 for counter-clockwise
        protected ICardPlacementStrategy CardPlacementStrategy { get; set; }

        protected AbstractGame()
        {
            PlayerDecks = new List<PlayerDeck>();
            Players = new Dictionary<string, string>();
            TopCard = UnoCard.GenerateCard();
            IsStarted = false;
            CurrentPlayerIndex = 0;
            Direction = 1;
            CardPlacementStrategy = new UnoPlacementStrategy();
        }
        public void SetPlacementStrategy(ICardPlacementStrategy strategy)
        {
            CardPlacementStrategy = strategy;
        }
        public abstract void Start();
        public abstract void End();
        public abstract void DrawCard(string username);
        public abstract string PlayCard(string username, UnoCard card);
        protected abstract void NextPlayer();
    }
}
