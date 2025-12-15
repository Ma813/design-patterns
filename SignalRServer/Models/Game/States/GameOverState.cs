using SignalRServer.Models;

namespace SignalRServer.Models.Game.States
{
    public class GameOverState : IGameState
    {
        public async Task<string> HandleJoinRoom(AbstractGame game, string userName, string connectionId)
        {
            return "Cannot join. Game has ended. Please start a new game.";
        }

        public async Task<string> HandleStartGame(AbstractGame game)
        {
            Console.WriteLine($"[{game.RoomName}] GameOverState: HandleStartGame called");
            
            // Reset game state for new game
            game.ResetGameState();
            
            Console.WriteLine($"[{game.RoomName}] GameOverState: Game reset complete, returning 'Ready for new game'");
            return "Ready for new game";
        }

        public async Task<string> HandleDrawCard(AbstractGame game, string userName)
        {
            return "Cannot draw card. Game has ended.";
        }

        public async Task<string> HandlePlayCard(AbstractGame game, string userName, int cardIndex)
        {
            return "Cannot play card. Game has ended.";
        }

        public string GetStateName() => "GameOver";

        public bool CanTransitionTo(IGameState newState)
        {
            return newState is LobbyState;
        }
    }
}