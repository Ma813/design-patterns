using Microsoft.AspNetCore.SignalR;
using SignalRServer.Card;
using SignalRServer.Models;
using SignalRServer.Models.Game;

namespace SignalRServer.Expressions;

public static class SystemMessages
{
    public static async Task UserJoined(AbstractGame game, string connectionId, string userName, string roomName, IHubCallerClients clients)
    {
        string playerList = "Players in the room:\n";

        foreach (var player in game.Players)
        {
            playerList += $"- {player.Value}\n";
            if (player.Key != connectionId)
                clients.Client(player.Key).SendAsync("SystemMessage", $"{userName} has joined the room {roomName}").Wait();

        }
        foreach (var player in game.Players)
        {
            if (player.Key != connectionId)
                clients.Client(player.Key).SendAsync("SystemMessage", playerList).Wait();
        }
    }

    public static async Task GameStarted(AbstractGame game, string userName, string connectionId, IHubCallerClients clients)
    {
        foreach (var player in game.Players)
        {
            if (player.Key != connectionId)
                clients.Client(player.Key).SendAsync("SystemMessage", $"The game has been started by \"{userName}\"!").Wait();

            GameForSending gameForSeding = new GameForSending(game, player.Value);

            clients.Client(player.Key).SendAsync("SystemMessage", gameForSeding.ToConsoleString()).Wait();
        }
    }

    public static async Task CardPlayed(AbstractGame game, string userName, string connectionId, IHubCallerClients<IClientProxy> clients, bool ended)
    {
        foreach (var player in game.Players)
        {
            if (player.Key != connectionId)
                clients.Client(player.Key).SendAsync("SystemMessage", $"{userName} has played a card: {game.TopCard}").Wait();
            GameForSending gameForSeding = new GameForSending(game, player.Value);
            clients.Client(player.Key).SendAsync("SystemMessage", gameForSeding.ToConsoleString()).Wait();
        }
        if (ended)
        {
            game.TopCard = UnoCard.GenerateCard(true); // Reset top card to avoid confusion
            foreach (var player in game.Players)
            {
                if (player.Key != connectionId)
                    clients.Client(player.Key).SendAsync("SystemMessage", $"{userName} has won the game!").Wait();
            }
        }  
    }

    public static async Task CardDrawn(AbstractGame game, string userName, string connectionId, IHubCallerClients<IClientProxy> clients)
    {
        foreach (var player in game.Players)
        {
            if (player.Key != connectionId)
                clients.Client(player.Key).SendAsync("SystemMessage", $"{userName} has drawn a card.").Wait();
            GameForSending gameForSeding = new GameForSending(game, player.Value);
            clients.Client(player.Key).SendAsync("SystemMessage", gameForSeding.ToConsoleString()).Wait();
        }
    }
}