using System;
using SignalRServer.Models;

namespace SignalRServer.Models
{
    // Abstract factory
    public abstract class AbstractGameCreator
    {
        public abstract AbstractGame CreateGame(string gameMode = "Classic");
    }
}