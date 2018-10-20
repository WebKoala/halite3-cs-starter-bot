using System;
using System.Collections.Generic;
using System.Text;
using Halite3.hlt;

namespace MyBot
{
    public interface IStrategy
    {
        Command OrderShip(Game game, Ship ship);

        bool ShouldSpawn(Game game);
    }
}
