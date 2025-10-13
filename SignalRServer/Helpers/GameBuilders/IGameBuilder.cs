using SignalRServer.Models;
using System.Collections.Generic;

namespace SignalRServer.Builders;

public interface IGameBuilder
{
    // Step 1: Create a new game instance (e.g., Game or EndlessGame)
    void CreateNewGame();

    // Step 2: Initialize player collections
    void BuildPlayerCollections();

    // Step 3: Configure card factory and top card
    void BuildCardFactory(CardGeneratingMode cardGeneratingMode);

    // Step 4: Configure card placement strategy
    void BuildPlacementStrategy(StrategyType placementStrategy);

    // Step 5: Initialize game state flags
    void BuildInitialState();

    // Step 6: Return the fully built game
    AbstractGame GetResult();
}
