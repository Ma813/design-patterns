using SignalRServer.Models;

namespace SignalRServer.Models.Game.States
{
    public class PlayingState : IGameState
    {
        public async Task<string> HandleJoinRoom(AbstractGame game, string userName, string connectionId)
        {
            return "Cannot join. Game is already in progress.";
        }

        public async Task<string> HandleStartGame(AbstractGame game)
        {
            return "Game is already in progress";
        }

        public async Task<string> HandleDrawCard(AbstractGame game, string userName)
        {
            // Use your existing DrawCard method
            game.DrawCard(userName);
            return "OK";
        }

        public async Task<string> HandlePlayCard(AbstractGame game, string userName, int cardIndex)
        {
            var playerDeck = game.PlayerDecks.FirstOrDefault(pd => pd.Username == userName);
            if (playerDeck == null)
            {
                return "Player not found";
            }

            if (cardIndex >= playerDeck.Cards.Count)
            {
                return "Invalid card index";
            }

            // Use your existing PlayCard method
            string result = game.PlayCard(userName, playerDeck.Cards[cardIndex]);
            return result;
        }

        public string GetStateName() => "Playing";

        public bool CanTransitionTo(IGameState newState)
        {
            return newState is GameOverState;
        }
    }
}