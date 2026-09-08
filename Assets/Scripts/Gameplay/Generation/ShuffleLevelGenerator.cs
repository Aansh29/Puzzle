using Puzzle.Gameplay.Grid;
using System;

namespace Puzzle.Gameplay.Generation
{
    public sealed class ShuffleLevelGenerator : ILevelGenerator
    {
        private readonly Random random;

        public ShuffleLevelGenerator()
        {
            random = new Random();
        }

        public void Generate(GridModel gridModel, int moveCount)
        {
            if (gridModel == null)
            {
                throw new ArgumentNullException(nameof(gridModel));
            }

            if (moveCount < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(moveCount));
            }

            GridDirection? previousDirection = null;

            for (int i = 0; i < moveCount; i++)
            {
                GridDirection direction;

                do
                {
                    direction = GetRandomDirection();
                }
                while (previousDirection.HasValue &&
                       direction == GetOppositeDirection(previousDirection.Value));

                if (!gridModel.TryMove(
                        direction,
                        out _,
                        out _,
                        out _))
                {
                    i--;

                    continue;
                }

                previousDirection = direction;
            }
        }

        private GridDirection GetRandomDirection()
        {
            return (GridDirection)random.Next(0, 4);
        }

        private GridDirection GetOppositeDirection(GridDirection direction)
        {
            switch (direction)
            {
                case GridDirection.Up:
                    return GridDirection.Down;

                case GridDirection.Down:
                    return GridDirection.Up;

                case GridDirection.Left:
                    return GridDirection.Right;

                case GridDirection.Right:
                    return GridDirection.Left;

                default:
                    throw new ArgumentOutOfRangeException(nameof(direction));
            }
        }
    }
}