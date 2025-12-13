using Microsoft.AspNetCore.Identity.Data;
using SignalRServer.Card;
using SignalRServer.Models.CardPlacementStrategies;
using SignalRServer.Visitors;

namespace SignalRServer.Models.Iterator;

public class PlayableCardContainer : IContainer<UnoCard>
{
    private readonly List<UnoCard> _cards;
    private readonly UnoCard _topCard;
    private readonly ICardPlacementStrategy _strategy;

    public PlayableCardContainer(List<UnoCard> cards, UnoCard topCard, ICardPlacementStrategy strategy)
    {
        _cards = cards;
        _topCard = topCard;
        _strategy = strategy;
    }

    public IIterator<UnoCard> CreateIterator()
    {
        return new PlayableCardIterator(_cards, _topCard, _strategy);
    }

    public int Count => _cards.Count;
    public UnoCard TopCard => _topCard;
    public ICardPlacementStrategy Strategy => _strategy;
}

public class PlayableCardIterator : IIterator<UnoCard>
{
    private readonly List<UnoCard> _cards;
    private readonly UnoCard _topCard;
    private readonly ICardPlacementStrategy _strategy;
    private int _currentIndex;
    private int _playableCount;

    public PlayableCardIterator(List<UnoCard> cards, UnoCard topCard, ICardPlacementStrategy strategy)
    {
        _cards = cards;
        _topCard = topCard;
        _strategy = strategy;

        Reset();
    }

    public void Reset()
    {
        _currentIndex = -1;
        _playableCount = 0;

        Next();
    }

    public void Next()
    {
        _currentIndex++;

        while(_currentIndex < _cards.Count)
        {
            if(_cards[_currentIndex].CanPlayOn(_topCard, _strategy))
            {
                _playableCount++;
                return;
            }

            _currentIndex++;
        }
    }

    public bool IsDone()
    {
        return _currentIndex >= _cards.Count;
    }

    public UnoCard Current()
    {
        if(_currentIndex >= 0 && _currentIndex < _cards.Count)
        {
            return _cards[_currentIndex];
        }

        return null!;
    }

    public bool HasNext()
    {
        for(int i = _currentIndex + 1; i < _cards.Count; i++)
        {
            if(_cards[i].CanPlayOn(_topCard, _strategy))
            {
                return true;
            }
        }

        return false;
    }

    public int CurrentIndex => _currentIndex;
    public int PlayableCount => _playableCount;

    public List<int> GetAllPlayableIndices()
    {
        var indices = new List<int>();
        for(int i = 0; i < _cards.Count; i++)
        {
            if(_cards[i].CanPlayOn(_topCard, _strategy)) indices.Add(i);
        }

        return indices;
    }
}