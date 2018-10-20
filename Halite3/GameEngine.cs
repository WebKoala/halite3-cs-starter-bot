using System;
using System.Collections.Generic;
using System.Text;
using Halite3.hlt;

namespace MyBot
{
    public class GameEngine
    {
        private readonly string _name;
        private readonly IStrategy _strategy;

        public GameEngine(string name, IStrategy strategy)
        {
            _name = name;
            _strategy = strategy;
        }

        internal void Start()
        {
            var game = new Game();
            // At this point "game" variable is populated with initial map data.
            // This is a good place to do computationally expensive start-up pre-processing.
            // As soon as you call "ready" function below, the 2 second per turn timer will start.
            game.Ready(_name);

            Log.LogMessage("Successfully created bot! My Player ID is " + game.myId);

            for (; ; )
            {
                game.UpdateFrame();
                var me = game.me;
                var gameMap = game.gameMap;

                var commandQueue = new List<Command>();

                foreach (var ship in me.ships.Values)
                {
                    commandQueue.Add(_strategy.OrderShip(game, ship));
                }

                if (_strategy.ShouldSpawn(game))
                {
                    commandQueue.Add(me.shipyard.Spawn());
                }
                game.EndTurn(commandQueue);
            }
        }
    }
}
