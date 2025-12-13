namespace SignalRServer.Models.Iterator;

/// <summary>
/// Aggregate that holds a collection of PlayerDecks for turn management.
/// Supports directional iteration (clockwise/counter-clockwise).
/// </summary>
public class PlayerTurnContainer : IContainer<PlayerDeck>
{
    private readonly List<PlayerDeck> _players;
    private int _direction;

    public PlayerTurnContainer(List<PlayerDeck> players, int direction = 1)
    {
        _players = players;
        _direction = direction; // 1 = clockwise, -1 = counter-clockwise
    }

    public IIterator<PlayerDeck> CreateIterator()
    {
        return new PlayerTurnIterator(this);
    }

    public int Count => _players.Count;
    public int Direction => _direction;
    
    public void SetDirection(int direction)
    {
        _direction = direction;
    }
    
    public void ReverseDirection()
    {
        _direction *= -1;
    }

    public PlayerDeck this[int index] => _players[index];
}

/// <summary>
/// Iterator for circular iteration through players.
/// Handles direction changes (reverse cards) and skipping players.
/// </summary>
public class PlayerTurnIterator : IIterator<PlayerDeck>
{
    private readonly PlayerTurnContainer _container;
    private int _currentIndex;
    private int _iterationCount; // Track how many times we've iterated

    public PlayerTurnIterator(PlayerTurnContainer container)
    {
        _container = container;
        _currentIndex = 0;
        _iterationCount = 0;
    }

    /// <summary>
    /// Reset to the first player.
    /// </summary>
    public void Reset()
    {
        _currentIndex = 0;
        _iterationCount = 0;
    }

    /// <summary>
    /// Move to the next player based on current direction.
    /// Handles circular wrapping.
    /// </summary>
    public void Next()
    {
        if (_container.Count == 0) return;
        
        _currentIndex = (_currentIndex + _container.Direction + _container.Count) % _container.Count;
        _iterationCount++;
    }

    /// <summary>
    /// Skip a number of players (useful for skip cards).
    /// </summary>
    public void Skip(int count = 1)
    {
        for (int i = 0; i < count; i++)
        {
            Next();
        }
    }

    /// <summary>
    /// Returns true if we've gone through all players once.
    /// Note: In a circular iterator, this is based on iteration count.
    /// </summary>
    public bool IsDone()
    {
        return _iterationCount >= _container.Count;
    }

    /// <summary>
    /// Returns the current player.
    /// </summary>
    public PlayerDeck Current()
    {
        if (_container.Count == 0) return null!;
        return _container[_currentIndex];
    }

    /// <summary>
    /// In a circular iterator, there's always a next player if there are players.
    /// </summary>
    public bool HasNext()
    {
        return _container.Count > 0;
    }

    /// <summary>
    /// Get the current player index.
    /// </summary>
    public int CurrentIndex => _currentIndex;

    /// <summary>
    /// Set the current index directly (useful for game state restoration).
    /// </summary>
    public void SetCurrentIndex(int index)
    {
        if (index >= 0 && index < _container.Count)
        {
            _currentIndex = index;
        }
    }

    /// <summary>
    /// Peek at the next player without moving the iterator.
    /// </summary>
    public PlayerDeck PeekNext()
    {
        if (_container.Count == 0) return null!;
        int nextIndex = (_currentIndex + _container.Direction + _container.Count) % _container.Count;
        return _container[nextIndex];
    }

    /// <summary>
    /// Reset iteration count but keep current position.
    /// </summary>
    public void ResetIterationCount()
    {
        _iterationCount = 0;
    }

    /// <summary>
    /// Get all players in turn order starting from current player.
    /// </summary>
    public List<PlayerDeck> GetPlayersInTurnOrder()
    {
        var result = new List<PlayerDeck>();
        if (_container.Count == 0) return result;

        int startIndex = _currentIndex;
        int index = startIndex;
        
        do
        {
            result.Add(_container[index]);
            index = (index + _container.Direction + _container.Count) % _container.Count;
        } while (index != startIndex);

        return result;
    }
}