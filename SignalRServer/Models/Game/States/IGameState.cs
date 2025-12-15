using SignalRServer.Models;

namespace SignalRServer.Models.Game.States
{
    public interface IGameState
    {
        Task<string> HandleJoinRoom(AbstractGame game, string userName, string connectionId);
        Task<string> HandleStartGame(AbstractGame game);
        Task<string> HandleDrawCard(AbstractGame game, string userName);
        Task<string> HandlePlayCard(AbstractGame game, string userName, int cardIndex);
        string GetStateName();
        bool CanTransitionTo(IGameState newState);
    }
}