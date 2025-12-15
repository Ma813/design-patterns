using SignalRServer.Models;

namespace SignalRServer.Models.Game.States
{
    public class InactiveState : IGameState
    {
        public async Task<string> HandleJoinRoom(AbstractGame game, string userName, string connectionId)
        {
            game.Players[connectionId] = userName;
            return "Player joined. Room is now active.";
        }

        public async Task<string> HandleStartGame(AbstractGame game)
        {
            return "Cannot start game. No players joined yet.";
        }

        public async Task<string> HandleDrawCard(AbstractGame game, string userName)
        {
            return "Cannot draw card. Game is inactive.";
        }

        public async Task<string> HandlePlayCard(AbstractGame game, string userName, int cardIndex)
        {
            return "Cannot play card. Game is inactive.";
        }

        public string GetStateName() => "Inactive";

        public bool CanTransitionTo(IGameState newState)
        {
            return newState is LobbyState;
        }
    }
}