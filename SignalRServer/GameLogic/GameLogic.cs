using Microsoft.AspNetCore.SignalR;
using SignalRServer.Models;
using SignalRServer.Helpers;

namespace SignalRServer.GameLogic;

public static class GameHandler
{
    private static Logger logger = Logger.GetInstance();

    public static async Task StartGame(Game game, IHubCallerClients Clients)
    {
        game.Start();
        foreach (var player in game.Players)
        {
            GameDto gameForSeding = new(game, player.Value);
            await Clients.Client(player.Key).SendAsync("GameStarted", gameForSeding);
        }

        logger.LogInfo($"Game started with players: {string.Join(", ", game.Players.Values)}");
    }

    public static async Task DrawCard(Game game, string userName, IHubCallerClients Clients)
    {
        PlayerDeck? playerDeck = game.PlayerDecks.FirstOrDefault(d => d.Username == userName);
        if (playerDeck == null) return;

        var random = new Random();
        BaseCard newCard = new NumberCard(((Colors)random.Next(0, 5)).ToString(), random.Next(0, 10));
        playerDeck.Cards.Add(newCard);
        game.NextPlayer();

        foreach (var player in game.Players)
        {
            GameDto gameForSending = new(game, player.Value);
            await Clients.Client(player.Key).SendAsync("GameStatus", gameForSending);
        }

        logger.LogInfo($"{userName} drew a card ({newCard.Color} {newCard.Name}).");
    }

    public static async Task PlayCard(Game game, string userName, BaseCard card, IHubCallerClients Clients, HubCallerContext context)
    {
        PlayerDeck? playerDeck = game.PlayerDecks.FirstOrDefault(d => d.Username == userName);
        if (playerDeck == null) return;

        if (game.PlayerDecks[game.currentPlayerIndex].Username != userName)
        {
            await Clients.Client(context.ConnectionId).SendAsync("Error", "It's not your turn.");
            return; // Not this player's turn
        }

        if (!card.CanPlay(game.topCard))
        {
            await Clients.Client(context.ConnectionId).SendAsync("Error", "You cannot play this card.");
            return; // Invalid move
        }

        game.topCard = card;
        playerDeck.Cards.Remove(card);

        game.NextPlayer();

        foreach (var player in game.Players)
        {
            GameDto gameForSending = new(game, player.Value);
            await Clients.Client(player.Key).SendAsync("GameStatus", gameForSending);
        }

        logger.LogInfo($"{userName} played a {card.Color} {card.Name} card.");
    }
}
