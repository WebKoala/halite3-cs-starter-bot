using System;
using System.Collections.Generic;

namespace Halite3.hlt
{
    public class GameMap
    {
        public readonly MapCell[][] cells;
        public readonly int height;
        public readonly int width;

        public GameMap(int width, int height)
        {
            this.width = width;
            this.height = height;

            cells = new MapCell[height][];
            for (var y = 0; y < height; ++y)
            {
                cells[y] = new MapCell[width];
            }
        }

        public MapCell At(Position position)
        {
            var normalized = normalize(position);
            return cells[normalized.y][normalized.x];
        }

        public MapCell At(Entity entity)
        {
            return At(entity.position);
        }

        public int CalculateDistance(Entity source, Entity target)
        {
            return CalculateDistance(source.position, target.position);
        }

        public int CalculateDistance(Position source, Position target)
        {
            var normalizedSource = normalize(source);
            var normalizedTarget = normalize(target);

            var dx = Math.Abs(normalizedSource.x - normalizedTarget.x);
            var dy = Math.Abs(normalizedSource.y - normalizedTarget.y);

            var toroidal_dx = Math.Min(dx, width - dx);
            var toroidal_dy = Math.Min(dy, height - dy);

            return toroidal_dx + toroidal_dy;
        }

        public Position normalize(Position position)
        {
            var x = (position.x % width + width) % width;
            var y = (position.y % height + height) % height;
            return new Position(x, y);
        }

        public List<Direction> GetUnsafeMoves(Entity source, Entity destination)
        {
            return GetUnsafeMoves(source.position, destination.position);
        }

        public List<Direction> GetUnsafeMoves(Position source, Position destination)
        {
            var possibleMoves = new List<Direction>();

            var normalizedSource = normalize(source);
            var normalizedDestination = normalize(destination);

            var dx = Math.Abs(normalizedSource.x - normalizedDestination.x);
            var dy = Math.Abs(normalizedSource.y - normalizedDestination.y);
            var wrapped_dx = width - dx;
            var wrapped_dy = height - dy;

            if (normalizedSource.x < normalizedDestination.x)
            {
                possibleMoves.Add(dx > wrapped_dx ? Direction.WEST : Direction.EAST);
            }
            else if (normalizedSource.x > normalizedDestination.x)
            {
                possibleMoves.Add(dx < wrapped_dx ? Direction.WEST : Direction.EAST);
            }

            if (normalizedSource.y < normalizedDestination.y)
            {
                possibleMoves.Add(dy > wrapped_dy ? Direction.NORTH : Direction.SOUTH);
            }
            else if (normalizedSource.y > normalizedDestination.y)
            {
                possibleMoves.Add(dy < wrapped_dy ? Direction.NORTH : Direction.SOUTH);
            }

            return possibleMoves;
        }

        public Direction NaiveNavigate(Ship ship, Entity destination)
        {
            return NaiveNavigate(ship, destination.position);
        }
        public Direction NaiveNavigate(Ship ship, Position destination)
        {
            // getUnsafeMoves normalizes for us
            foreach (var direction in GetUnsafeMoves(ship.position, destination))
            {
                var targetPos = ship.position.DirectionalOffset(direction);
                if (!At(targetPos).IsOccupied())
                {
                    At(targetPos).MarkUnsafe(ship);
                    return direction;
                }
            }
            return Direction.STILL;
        }

        public IEnumerable<(Position pos, Direction d)> GetNeighbors(Entity source)
        {
            return GetNeighbors(source.position);
        }

        public IEnumerable<(Position pos, Direction d)> GetNeighbors(Position position)
        {
            foreach (Direction d in Enum.GetValues(typeof(Direction)))
            {
                if (d == Direction.STILL)
                {
                    continue;
                }

                var neighbor = position.DirectionalOffset(d);
                if (IsWithinBounds(neighbor) && !At(neighbor).IsOccupied())
                {
                    yield return (neighbor, d);
                }
            }
        }

        private bool IsWithinBounds(Position p)
        {
            return p.x < width && p.x >= 0 &&
                   p.y < height && p.y >= 0;

        }
        

        public void _update()
        {
            for (var y = 0; y < height; ++y)
            {
                for (var x = 0; x < width; ++x)
                {
                    cells[y][x].ship = null;
                }
            }

            var updateCount = Input.ReadInput().GetInt();

            for (var i = 0; i < updateCount; ++i)
            {
                var input = Input.ReadInput();
                var x = input.GetInt();
                var y = input.GetInt();

                cells[y][x].halite = input.GetInt();
            }
        }

        public static GameMap _generate()
        {
            var mapInput = Input.ReadInput();
            var width = mapInput.GetInt();
            var height = mapInput.GetInt();

            var map = new GameMap(width, height);

            for (var y = 0; y < height; ++y)
            {
                var rowInput = Input.ReadInput();

                for (var x = 0; x < width; ++x)
                {
                    var halite = rowInput.GetInt();
                    map.cells[y][x] = new MapCell(new Position(x, y), halite);
                }
            }

            return map;
        }
    }
}