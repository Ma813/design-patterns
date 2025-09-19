using Microsoft.AspNetCore.SignalR;
using SignalRServer.Models;

namespace SignalRServer.Hubs
{
    public class ChatHub : Hub
    {
        private static readonly Dictionary<string, string> UserRooms = new Dictionary<string, string>();
        private static readonly Dictionary<string, Game> Games = new Dictionary<string, Game>(); // {roomName: Game}


        public async Task JoinRoom(string roomName, string userName)
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

            Game game;
            if (Games.ContainsKey(roomName)) game = Games[roomName];
            else
            {
                game = new Game();
                Games[roomName] = game;
            }

            if (game.isStarted)
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
            Game game = Games[roomName];
            game.Start();

            Dictionary<string, int> cardAmounts = new Dictionary<string, int>();
            foreach (PlayerDeck pDeck in game.PlayerDecks)
            {
                cardAmounts[pDeck.Username] = pDeck.Count;
            }

            UnoCard topCard = game.topCard;

            foreach (var player in game.Players)
            {
                PlayerDeck? deck = game.PlayerDecks.FirstOrDefault(d => d.Username == player.Value);
                await Clients.Client(player.Key).SendAsync("GameStarted", deck, topCard, cardAmounts);
            }
        }



        public async Task SendMessage(string roomName, string user, string message)
        {
            await Clients.Group(roomName).SendAsync("ReceiveMessage", user, message);
        }

        public async Task SendButtonPress(string roomName, string user, string buttonName)
        {
            await Clients.Group(roomName).SendAsync("ReceiveButtonPress", user, buttonName, DateTime.Now);
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

        // Add this method to get current connection status
        public string GetConnectionId() => Context.ConnectionId;
    }
}