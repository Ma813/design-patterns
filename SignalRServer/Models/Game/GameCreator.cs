using System;
using SignalRServer.Models;

namespace SignalRServer.Models
{
    // Abstract factory
    public class GameCreator : AbstractGameCreator
    {
        public override AbstractGame CreateGame(string gameMode = "Classic", string roomName = "DefaultRoom")
        {
            switch (gameMode)
            {
                case "Classic":
                    System.Console.WriteLine("Classic mode selected");
                    return new Game(roomName);
                // Future game modes can be added here
                case "Endless":
                    System.Console.WriteLine("Endless mode selected");
                    return new EndlessGame(roomName);
                case "DrawToMatch":
                    System.Console.WriteLine("DrawToMatch mode selected");
                    return new DrawToMatchGame(roomName);
                default:
                    throw new ArgumentException("Invalid game mode");
            }
        }
    }
}