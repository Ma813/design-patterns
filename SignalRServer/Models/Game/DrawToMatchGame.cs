using Microsoft.AspNetCore.SignalR;
using SignalRServer.Card;
using SignalRServer.Models.Commands;

namespace SignalRServer.Models.Game;

public class DrawToMatchGame : AbstractGame
{
    public DrawToMatchGame(string roomName = "DefaultRoom") : base(roomName)
    {
        RoomName = roomName;
    }

    public override void Start(IHubCallerClients? client = null)
    {
        IsStarted = true;
        foreach (var player in Players)
        {
            PlayerDeck deck = new(player.Value, Generator, client: client?.Client(player.Key));
            PlayerDecks.Add(deck);
        }
    }

    public override void End()
    {
        IsStarted = false;
        PlayerDecks.Clear();
        TopCard = UnoCard.GenerateCard();
        CurrentPlayerIndex = 0;
        Direction = 1;
    }

    public override void DrawCard(string username)
    {
        var playerDeck = PlayerDecks.FirstOrDefault(pd => pd.Username == username);
        if (playerDeck == null) return;

        playerDeck.ExecuteCommand(new DrawCardCommand(this, playerDeck));

        // NextPlayer();
        // In DrawToMatch, the player does not change after drawing a card
    }

    public override string UndoCard(string username)
    {
        var playerDeck = PlayerDecks.FirstOrDefault(pd => pd.Username == username);
        if (playerDeck == null) return "Player not found";

        playerDeck.Undo();
        //playerDeck.ExecuteCommand(new UndoCardCommand(this, playerDeck));
        return "OK";
    }

    public override void NextPlayer(Action action)
    {
        if (action == Action.draw)
        {
            // In DrawToMatch, if the action was drawing a card, the same player plays again
            return;
        }
        CurrentPlayerIndex = (CurrentPlayerIndex + Direction + PlayerDecks.Count) % PlayerDecks.Count;
    }

    public override void NextDrawCard()
    {
        var next = (CurrentPlayerIndex + Direction + PlayerDecks.Count) % PlayerDecks.Count;
        PlayerDecks[next].ExecuteCommand(new DrawCardCommand(this, PlayerDecks[next]));
    }
}
