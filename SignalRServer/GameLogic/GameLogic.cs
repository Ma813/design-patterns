using Microsoft.AspNetCore.SignalR;
using SignalRServer.Models;

namespace SignalRServer.GameLogic;

public static class GameHandler
{
    public static async Task StartGame(Game game, IHubCallerClients Clients)
    {
        game.Start();
        foreach (var player in game.Players)
        {
            GameDto gameForSeding = new(game, player.Value);
            await Clients.Client(player.Key).SendAsync("GameStarted", gameForSeding);
        }
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
    }
}
