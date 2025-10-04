using System;
using SignalRServer.Models;

namespace SignalRServer.Models
{
    // Abstract factory
    public class GameCreator : AbstractGameCreator
    {
        public override AbstractGame CreateGame(string gameMode = "Classic")
        {
            switch (gameMode)
            {
                case "Classic":
                    System.Console.WriteLine("Classic mode selected");
                    return new Game();
                // Future game modes can be added here
                case "Endless":
                    System.Console.WriteLine("Endless mode selected");
                    return new EndlessGame();
                case "DrawToMatch":
                    System.Console.WriteLine("DrawToMatch mode selected");
                    return new DrawToMatchGame();
                default:
                    throw new ArgumentException("Invalid game mode");
            }
        }
    }
}