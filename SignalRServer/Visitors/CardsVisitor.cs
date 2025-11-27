using Microsoft.AspNetCore.SignalR;
using SignalRServer.Models;

namespace SignalRServer.Visitors;

public class CardsVisitor : IDeckVisitor
{
    public void Visit(PlayerDeck deck)
    {
        deck._client?.SendAsync("UpdateCards", deck.Cards);
    }
}