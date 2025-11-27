using Microsoft.AspNetCore.SignalR;
using SignalRServer.Card;
using SignalRServer.Models;
using SignalRServer.Models.Commands;
using SignalRServer.Visitors;

namespace SignalRServer.Visitors;

public class LoggerVisitor : IDeckVisitor
{
    Logger _logger = Logger.GetInstance();
    public void Visit(PlayerDeck deck)
    {
        _logger.LogInfo($"Player: {deck.Username}, Cards: {string.Join(", ", deck.Cards.Select(c => c.Name))}");
    }
}