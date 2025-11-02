using SignalRServer.Card;
using SignalRServer.Models.Commands;

namespace SignalRServer.Models;

public class PlayerDeck
{
    public List<UnoCard> Cards { get; private set; }
    public string Username { get; private set; }
    public CommandHistory history = new(); 

    public PlayerDeck(string username)
    {
        Cards = [];
        Username = username;
        Cards.Add(UnoCard.GenerateSuperCard());
        for (int i = 0; i < 4; i++)
        {
            UnoCard card = UnoCard.GenerateCard();
            Cards.Add(card);
        }
    }

    public void AddCard(UnoCard card)
    {
        Cards.Add(card);
    }

    public bool RemoveCard(UnoCard card)
    {
        return Cards.Remove(card);
    }

    public void ExecuteCommand(Command command)
    {
        if (command.Execute())
        {
            history.Push(command);
        }
    }

    // Take the most recent command from history and run its undo method
    public void Undo()
    {
        Command? command = history.Pop();
        command?.Undo();
    }

    public int Count => Cards.Count;
}
