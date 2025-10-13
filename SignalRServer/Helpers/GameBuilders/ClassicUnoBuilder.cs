using SignalRServer.Models;
using SignalRServer.Helpers;
using System.Collections.Generic;

namespace SignalRServer.Builders;

public class ClassicUnoBuilder : IGameBuilder
{
    private Game _game;

    public void CreateNewGame()
    {
        _game = new Game(); // No params now
    }

    public void BuildPlayerCollections()
    {
        _game.PlayerDecks = new List<PlayerDeck>();
        _game.Players = new Dictionary<string, string>();
    }

    public void BuildCardFactory()
    {
        _game.cardFactory = CardFactoryType.Normal;
        _game.TopCard = _game.cardFactory.GenerateCard();
    }

    public void BuildPlacementStrategy()
    {
        _game.cardPlacementStrategy = StrategyType.Normal;
    }

    public void BuildInitialState()
    {
        _game.IsStarted = false;
        _game.CurrentPlayerIndex = 0;
        _game.Direction = 1;
    }

    public AbstractGame GetResult() => _game;
}
