using System;
using System.Collections.Generic;
using System.IO;

namespace Halite3.hlt
{
    /// <summary>
    ///     The game object holds all metadata pertinent to the game and all its contents
    /// </summary>
    public class Game
    {
        public readonly GameMap gameMap;
        public readonly Player me;
        public readonly PlayerId myId;
        public readonly List<Player> players = new List<Player>();
        public int turnNumber;

        /// <summary>
        ///     Initiates a game object collecting all start-state instances for the contained items for pre-game.
        ///     Also sets up basic logging.
        /// </summary>
        public Game()
        {
            //Grab constants JSON
            Constants.LoadConstants(Input.ReadLine());

            var inputs = Input.ReadInput();
            var numPlayers = inputs.GetInt();
            myId = new PlayerId(inputs.GetInt());

            Log.Initialize(new StreamWriter(string.Format("bot-{0}.log", myId)));

            for (var i = 0; i < numPlayers; i++)
            {
                players.Add(Player._generate());
            }

            me = players[myId.id];
            gameMap = GameMap._generate();
        }

        public void Ready(string name)
        {
            Console.WriteLine(name);
        }

        public void UpdateFrame()
        {
            turnNumber = Input.ReadInput().GetInt();
            Log.LogMessage("=============== TURN " + turnNumber + " ================");

            for (var i = 0; i < players.Count; ++i)
            {
                var input = Input.ReadInput();

                var currentPlayerId = new PlayerId(input.GetInt());
                var numShips = input.GetInt();
                var numDropoffs = input.GetInt();
                var halite = input.GetInt();

                players[currentPlayerId.id]._update(numShips, numDropoffs, halite);
            }

            gameMap._update();

            foreach (var player in players)
            {
                foreach (var ship in player.ships.Values)
                {
                    gameMap.At(ship).MarkUnsafe(ship);
                }

                gameMap.At(player.shipyard).structure = player.shipyard;

                foreach (var dropoff in player.dropoffs.Values)
                {
                    gameMap.At(dropoff).structure = dropoff;
                }
            }
        }

        public void EndTurn(IEnumerable<Command> commands)
        {
            foreach (var command in commands)
            {
                Console.Write(command.command);
                Console.Write(' ');
            }

            Console.WriteLine();
        }
    }
}