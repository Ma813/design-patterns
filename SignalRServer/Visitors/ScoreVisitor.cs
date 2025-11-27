using SignalRServer.Models;
using SignalRServer.Card;
using Microsoft.AspNetCore.SignalR;

namespace SignalRServer.Visitors;

public class ScoreVisitor : IDeckVisitor
{
    public void Visit(PlayerDeck deck)
    {
        int TotalScore = 0;
        foreach (var card in deck.Cards)
        {
            if (card is NumberCard numberCard)
            {
                TotalScore += numberCard.Digit;
            }
            else if (card is PowerCard)
            {
                TotalScore += 20;
            }

            deck._client?.SendAsync("UpdateScore", TotalScore);
        }
    }
}