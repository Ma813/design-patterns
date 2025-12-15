using SignalRServer.Models;

namespace SignalRServer.Models.Game.States
{
    public class LobbyState : IGameState
    {
        public async Task<string> HandleJoinRoom(AbstractGame game, string userName, string connectionId)
        {
            if (game.Players.ContainsValue(userName))
            {
                return "Player already in game";
            }

            game.Players[connectionId] = userName;
            Console.WriteLine($"[{game.RoomName}] LobbyState: Player {userName} joined. Total players: {game.Players.Count}");
            return "Player joined successfully";
        }

        public async Task<string> HandleStartGame(AbstractGame game)
        {
            Console.WriteLine($"[{game.RoomName}] LobbyState: HandleStartGame called. Players: {game.Players.Count}");
            
            if (game.Players.Count < 2)
            {
                return "Need at least 2 players to start";
            }
            
            Console.WriteLine($"[{game.RoomName}] LobbyState: Sufficient players, returning OK");
            return "OK"; // This will trigger the state transition to Playing
        }

        public async Task<string> HandleDrawCard(AbstractGame game, string userName)
        {
            return "Cannot draw card. Game hasn't started yet.";
        }

        public async Task<string> HandlePlayCard(AbstractGame game, string userName, int cardIndex)
        {
            return "Cannot play card. Game hasn't started yet.";
        }

        public string GetStateName() => "Lobby";

        public bool CanTransitionTo(IGameState newState)
        {
            return newState is PlayingState;
        }
    }
}