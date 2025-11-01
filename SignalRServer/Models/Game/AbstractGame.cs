using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.SignalR;
using SignalRServer.Models.CardPlacementStrategies;
namespace SignalRServer.Models
{
    public abstract class AbstractGame
    {
        public string RoomName { get; set; }
        public List<PlayerDeck> PlayerDecks { get; set; }
        public Dictionary<string, string> Players { get; set; } // {connectionId: username}
        public UnoCard TopCard { get; set; }
        public bool IsStarted { get; set; }
        public int CurrentPlayerIndex { get; set; }
        public int Direction { get; set; } // 1 for clockwise, -1 for counter-clockwise
        public List<BotClient> Bots { get; set; } = new List<BotClient>();
        protected ICardPlacementStrategy CardPlacementStrategy { get; set; }

        public IHubCallerClients<IClientProxy>? Clients { get; set; } = null;

        protected AbstractGame(string roomName)
        {
            RoomName = roomName;
            PlayerDecks = new List<PlayerDeck>();
            Players = new Dictionary<string, string>();
            TopCard = UnoCard.GenerateCard();
            IsStarted = false;
            CurrentPlayerIndex = 0;
            Direction = 1;
            CardPlacementStrategy = new UnoPlacementStrategy();

        }

        public void setClients(IHubCallerClients<IClientProxy> clients)
        {
            Clients = clients;
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

        public void NotifyBots()
        {
            foreach (var bot in Bots)
            {
                bot.getNotified();
            }
        }

        public string AddBot()
        {
            var bot = new BotClient($"Bot{Bots.Count + 1}", this);
            Bots.Add(bot);
            return bot.userName;
        }
    }
}
