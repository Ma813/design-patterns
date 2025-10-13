using SignalRServer.Models;
using SignalRServer.Helpers;
using System.Collections.Generic;

namespace SignalRServer.Builders;

public class AdjacentNumberEndlessBuilder : IGameBuilder
{
    private EndlessGame _game;

    public void CreateNewGame()
    {
        _game = new EndlessGame();
    }

    public void BuildPlayerCollections()
    {
        _game.PlayerDecks = new List<PlayerDeck>();
        _game.Players = new Dictionary<string, string>();
    }

    public void BuildCardFactory()
    {
        _game.cardFactory = CardFactoryCreator.GetFactory(CardFactoryType.SameColor);
        _game.TopCard = _game.cardFactory.GenerateCard();
    }

    public void BuildPlacementStrategy()
    {
        _game.cardPlacementStrategy = StrategyCreator.GetStrategy(StrategyType.AdjacentNumber);
    }

    public void BuildInitialState()
    {
        _game.IsStarted = false;
        _game.CurrentPlayerIndex = 0;
        _game.Direction = 1;
    }

    public AbstractGame GetResult() => _game;
}
