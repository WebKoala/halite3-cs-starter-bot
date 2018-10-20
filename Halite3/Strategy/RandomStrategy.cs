using System.Linq;
using Halite3.hlt;

namespace MyBot
{
    public class RandomStrategy : IStrategy
    {
        public bool ShouldSpawn(Game game)
        {
            if (
                game.turnNumber <= 200 &&
                game.me.halite >= Constants.SHIP_COST &&
                !game.gameMap.At(game.me.shipyard).IsOccupied())
            {
                return true;
            }

            return false;
        }

        public Command OrderShip(Game game, Ship ship)
        {
            if (ship.IsFilledAboveThreshold(0.7f))
            {
                var direction = game.gameMap.NaiveNavigate(ship, game.me.shipyard);
                return ship.Move(direction);
            }
            else if (game.gameMap.At(ship).halite < Constants.MAX_HALITE / 10)
            {
                var maxHalite = game.gameMap.At(ship).halite;
                Direction? candidate = null;
                foreach (var (pos, d) in game.gameMap.GetNeighbors(ship))
                {
                    var halite = game.gameMap.At(pos).halite;
                    if (!candidate.HasValue || halite > maxHalite)
                    {
                        candidate = d;
                        maxHalite = halite;
                    }
                }
                return ship.Move(candidate ?? Direction.STILL);
            }
            else
            {
                return ship.StayStill();
            }
        }
    }
}
