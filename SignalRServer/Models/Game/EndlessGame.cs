using SignalRServer.Card;
using SignalRServer.Models.Commands;

namespace SignalRServer.Models.Game;

public class EndlessGame : AbstractGame
{
    public EndlessGame(string roomName = "DefaultRoom") : base(roomName)
    {
        RoomName = roomName;
    }


    public override void Start()
    {
        IsStarted = true;
        foreach (var player in Players.Values)
        {
            PlayerDeck deck = new(player);
            PlayerDecks.Add(deck);
        }
    }

    public override void End()
    {
        // Endless game does not end
    }

    public override void DrawCard(string username)
    {
        var playerDeck = PlayerDecks.FirstOrDefault(pd => pd.Username == username);
        if (playerDeck == null) return;

        playerDeck.ExecuteCommand(new DrawCardCommand(this, playerDeck));
    }

    public override string PlayCard(string username, UnoCard card)
    {
        var playerDeck = PlayerDecks.FirstOrDefault(pd => pd.Username == username);

        if (playerDeck == null) return "Player not found";
        if (playerDeck != PlayerDecks[CurrentPlayerIndex]) return "Not your turn";
        if (!card.CanPlayOn(TopCard, CardPlacementStrategy)) return "Card cannot be played on top of current top card";

        playerDeck.ExecuteCommand(new PlayCardCommand(this, playerDeck, card, CardPlacementStrategy));
        if (playerDeck.Count == 0)
        {
            End();
            return "WIN";
        }
        return "OK";
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
        CurrentPlayerIndex = (CurrentPlayerIndex + Direction + PlayerDecks.Count) % PlayerDecks.Count;
    }

    public override void NextDrawCard()
    {
        var next = (CurrentPlayerIndex + Direction + PlayerDecks.Count) % PlayerDecks.Count;
        PlayerDecks[next].ExecuteCommand(new DrawCardCommand(this, PlayerDecks[next]));
    }
}
