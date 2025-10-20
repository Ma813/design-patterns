using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using SignalRServer.Models;
using SignalRServer.Models.CardPlacementStrategies;
namespace SignalRServer.Hubs
{
    public class GameHub : Hub
    {
        private static readonly Dictionary<string, string> UserRooms = new Dictionary<string, string>();
        private static readonly Dictionary<string, AbstractGame> Games = new Dictionary<string, AbstractGame>(); // {roomName: Game}
        
        private static readonly Dictionary<string, string> UsernameToConnectionId = new Dictionary<string, string>();

        IAnnoyingEffects annoyingFlashbang = new AnnoyingFlashbang();
        IAnnoyingEffects annoyingSoundEffect = new AnnoyingSoundEffect();


        SoundEffectAdaptee annoyingSoundAdaptee = new SoundEffectAdaptee();

        AbstractGameCreator gameFactory = new GameCreator();

        public async Task JoinRoom(string roomName, string userName, string gameMode = "Classic", string cardPlacementStrategy = "UnoPlacementStrategy")
        {
            var connectionId = Context.ConnectionId;

            // Remove user from previous room if any
            if (UserRooms.ContainsKey(connectionId))
            {
                await Groups.RemoveFromGroupAsync(connectionId, UserRooms[connectionId]);
            }

            if (UserRooms.ContainsKey(connectionId) && UserRooms[connectionId] == roomName) //player is already in room
            {
                return;
            }

            AbstractGame game;
            if (Games.ContainsKey(roomName)) game = Games[roomName];
            else
            {
                game = gameFactory.CreateGame(gameMode);
                Games[roomName] = game;
            }

            UsernameToConnectionId[userName] = connectionId;


            ICardPlacementStrategy strategy = cardPlacementStrategy switch
            {
                "AdjacentNumberPlacementStrategy" => new AdjacentNumberPlacementStrategy(),
                "ColorOnlyPlacementStrategy" => new ColorOnlyPlacementStrategy(),
                "NumberOnlyPlacementStrategy" => new NumberOnlyPlacementStrategy(),
                "UnoPlacementStrategy" => new UnoPlacementStrategy()
            };

            //  Set strategy on the game. If this isn't called or compatible, the strategy will be the standard Uno rule
            game.SetPlacementStrategy(strategy);



            if (game.IsStarted)
            {
                return; //Should return an errror later on
            }

            game.Players[Context.ConnectionId] = userName;

            // Add user to new room
            UserRooms[connectionId] = roomName;
            await Groups.AddToGroupAsync(connectionId, roomName);

            // Send only the array of usernames to the group, not the full dictionary
            var usernames = game.Players.Values.ToArray();
            await Clients.Group(roomName).SendAsync("UserJoined", usernames);
        }

        public async Task StartGame(string roomName, string userName)
        {
            AbstractGame game = Games[roomName];
            game.Start();

            foreach (var player in game.Players)
            {
                GameForSending gameForSeding = new GameForSending(game, player.Value);

                await Clients.Client(player.Key).SendAsync("GameStarted", gameForSeding);
            }
        }

        public async Task DrawCard(string roomName, string userName)
        {
            AbstractGame game = Games[roomName];
            game.DrawCard(userName);

            await notifyPlayers(game);
        }

        public async Task PlayCard(string roomName, string userName, UnoCard card)
        {
            AbstractGame game = Games[roomName];

            string result = game.PlayCard(userName, card);
            if (result == "WIN")
            {
                await Clients.Group(roomName).SendAsync("GameEnded", $"{userName} has won the game!");
                return;
            }
            if (result != "OK")
            {
                await Clients.Caller.SendAsync("Error", result);
                return;
            }

            await notifyPlayers(game);
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            var connectionId = Context.ConnectionId;
            if (UserRooms.TryGetValue(connectionId, out var roomName))
            {
                await Clients.Group(roomName).SendAsync("UserLeft", $"{connectionId} left the room");
                UserRooms.Remove(connectionId);
            }

            await base.OnDisconnectedAsync(exception);
        }

        public async Task AnnoyPlayers(string roomName, string message)
        {
            IClientProxy playerGroup = Clients.Group(roomName);

            IAnnoyingEffects annoyingEffect;

            switch (message)
            {
                case "flashbang":
                    annoyingEffect = annoyingFlashbang;
                    break;
                case "soundeffect":
                    annoyingEffect = annoyingSoundEffect;
                    break;
                default:
                    return;
            }

            await annoyingEffect.Annoy(playerGroup, Clients.Caller);
        }

        public async Task AnnoyPlayer(string roomName, string player, string message)
        {
            string playerId = UsernameToConnectionId[player];
            IClientProxy singlePlayer = Clients.Client(playerId);
            
            IAnnoyingEffects annoyingEffect;

            switch (message)
            {
                case "flashbang":
                    annoyingEffect = annoyingFlashbang;
                    break;
                case "soundeffect":
                    annoyingEffect = annoyingSoundEffect;
                    break;
                default:
                    return;
            }

            await annoyingEffect.Annoy(singlePlayer, Clients.Caller);
        }

        public async Task ToggleMutePlayer(string roomname, string player)
        {
            string playerId = UsernameToConnectionId[player];
            IClientProxy muted = Clients.Client(playerId);
            IClientProxy muting = Clients.Caller;
            await annoyingSoundAdaptee.ToggleMutePlayer(muted, muting);
        }

        private async Task notifyPlayers(AbstractGame game)
        {
            foreach (var player in game.Players)
            {
                GameForSending gameForSending = new GameForSending(game, player.Value);
                await Clients.Client(player.Key).SendAsync("GameStatus", gameForSending);
            }
        }

        // Add this method to get current connection status
        public string GetConnectionId() => Context.ConnectionId;
    }
}