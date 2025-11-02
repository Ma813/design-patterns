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
        if (playerDeck != null)
        {
            UnoCard card = UnoCard.GenerateCard();
            playerDeck.AddCard(card);
            NotifyAll(Action.draw, card);
        }
        NextPlayer();
    }

    public override string PlayCard(string username, UnoCard card)
    {
        var playerDeck = PlayerDecks.FirstOrDefault(pd => pd.Username == username);

        if (playerDeck == null) return "Player not found";
        if (playerDeck != PlayerDecks[CurrentPlayerIndex]) return "Not your turn";
        if (!card.CanPlayOn(TopCard, CardPlacementStrategy)) return "Card cannot be played on top of current top card";

        playerDeck.RemoveCard(card);
        TopCard = card;
        NotifyAll(Action.place, card);
        if (playerDeck.Count == 0)
        {
            End();
            return "WIN";
        }
        NextPlayer();
        return "OK";
    }

    protected override void NextPlayer()
    {
        CurrentPlayerIndex = (CurrentPlayerIndex + Direction + PlayerDecks.Count) % PlayerDecks.Count;
    }
}
