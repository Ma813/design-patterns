using Microsoft.AspNetCore.SignalR;
using SignalRServer.Card;
using SignalRServer.Models.Commands;
using SignalRServer.Visitors;

namespace SignalRServer.Models;

public class PlayerDeck
{
    public List<UnoCard> Cards { get; private set; }
    public string Username { get; private set; }
    public CommandHistory history = new();   
     private readonly CardGenerator _generator;

    public ISingleClientProxy? _client;

    public PlayerDeck(string username, CardGenerator generator, int initialCardCount = 7, ISingleClientProxy? client = null)
    {
        Cards = [];
        Username = username;
        Cards.Add(UnoCard.GenerateSuperCard());
        _generator = generator;

        for (int i = 0; i < initialCardCount; i++)
        {
            UnoCard card = UnoCard.GenerateCard();
            Cards.Add(card);
        }
        
        _client = client;
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

    public void Accept(IDeckVisitor visitor)
    {
        visitor.Visit(this);
    } 


    public int Count => Cards.Count;
}
