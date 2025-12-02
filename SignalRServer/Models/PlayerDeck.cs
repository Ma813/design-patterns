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
    public ISingleClientProxy? _client;

    public PlayerDeck(string username, int initialCardCount = 6, ISingleClientProxy? client = null, bool testMode = false)// testMode = true to get test win scenarios
    {
        Cards = [];
        Username = username;
        _client = client;

        if (testMode)
        {
            // For testing purposes, generate cards which guarantee a win condition can be tested quickly
            Cards.Add(new NumberCard("red", 0));
            Cards.Add(new NumberCard("blue", 0));
            Cards.Add(new NumberCard("green", 0));
            Cards.Add(new NumberCard("yellow", 0));
            return;
        }

        Cards.Add(UnoCard.GenerateSuperCard());
        for (int i = 0; i < initialCardCount; i++)
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

    public void Accept(IDeckVisitor visitor)
    {
        visitor.Visit(this);
    }


    public int Count => Cards.Count;
}
