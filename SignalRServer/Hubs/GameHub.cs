using Microsoft.AspNetCore.SignalR;
using SignalRServer.Models;

namespace SignalRServer.Hubs
{
    public class GameHub : Hub
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

            foreach (var player in game.Players)
            {
                GameForSending gameForSeding = new GameForSending(game, player.Value);
                
                await Clients.Client(player.Key).SendAsync("GameStarted", gameForSeding);
            }
        }

        public async Task DrawCard(string roomName, string userName)
        {
            Game game = Games[roomName];
            PlayerDeck? playerDeck = game.PlayerDecks.FirstOrDefault(d => d.Username == userName);
            if (playerDeck == null) return;

            UnoCard newCard = UnoCard.GenerateCard();
            playerDeck.Cards.Add(newCard);
            game.NextPlayer();
            
            foreach (var player in game.Players)
            {
                GameForSending gameForSending = new GameForSending(game, player.Value);
                await Clients.Client(player.Key).SendAsync("GameStatus", gameForSending);
            }
        }

        public async Task PlayCard(string roomName, string userName, UnoCard card)
        {
            Game game = Games[roomName];
            PlayerDeck? playerDeck = game.PlayerDecks.FirstOrDefault(d => d.Username == userName);
            if (playerDeck == null) return;

            if (game.PlayerDecks[game.currentPlayerIndex].Username != userName)
            {
                await Clients.Client(Context.ConnectionId).SendAsync("Error", "It's not your turn.");
                return; // Not this player's turn
            }

            if (!card.CanPlayOn(game.topCard))
            {
                await Clients.Client(Context.ConnectionId).SendAsync("Error", "You cannot play this card.");
                return; // Invalid move
            }

            game.topCard = card;
            playerDeck.Cards.Remove(card);

            game.NextPlayer();

            foreach (var player in game.Players)
            {
                GameForSending gameForSending = new GameForSending(game, player.Value);
                await Clients.Client(player.Key).SendAsync("GameStatus", gameForSending);
            }
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