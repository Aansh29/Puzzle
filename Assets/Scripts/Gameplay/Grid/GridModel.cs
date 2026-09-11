using Puzzle.Gameplay.History;
using System;
using System.Collections.Generic;

namespace Puzzle.Gameplay.Grid
{
    public sealed class GridModel
    {
        public const int EmptyCell = -1;

        private readonly int rows;
        private readonly int columns;
        private readonly int emptySpaces;
        private readonly int[,] cells;

        private readonly List<GridPosition> emptyPositions = new();

        public int Rows => rows;

        public int Columns => columns;

        public IReadOnlyList<GridPosition> EmptyPositions => emptyPositions;

        public GridModel(int rows, int columns, int emptySpaces = 1)
        {
            if (rows <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(rows));
            }

            if (columns <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(columns));
            }

            if (emptySpaces <= 0 || emptySpaces >= rows * columns)
            {
                throw new ArgumentOutOfRangeException(nameof(emptySpaces));
            }

            this.rows = rows;
            this.columns = columns;
            this.emptySpaces = emptySpaces;

            cells = new int[rows, columns];

            InitializeSolved();
        }

        public int GetCell(GridPosition position)
        {
            ValidatePosition(position);

            return cells[position.Row, position.Column];
        }

        public bool TryMove(GridDirection direction, out int movedValue, out GridPosition targetPosition, out GridPosition previousPosition)
        {
            movedValue = EmptyCell;
            targetPosition = default;
            previousPosition = default;

            GridPosition sourcePosition;
            GridPosition emptyPosition;

            if (!TryGetMovePositions(
                    direction,
                    out sourcePosition,
                    out emptyPosition))
            {
                return false;
            }

            movedValue = cells[sourcePosition.Row, sourcePosition.Column];

            previousPosition = sourcePosition;
            targetPosition = emptyPosition;

            cells[emptyPosition.Row, emptyPosition.Column] = movedValue;

            cells[sourcePosition.Row, sourcePosition.Column] = EmptyCell;

            ReplaceEmptyPosition(emptyPosition, sourcePosition);

            return true;
        }

        public void UndoMove(BoardMove move)
        {
            cells[move.TargetPosition.Row, move.TargetPosition.Column] = EmptyCell;

            cells[move.PreviousPosition.Row, move.PreviousPosition.Column] = move.MovedValue;

            ReplaceEmptyPosition(move.PreviousPosition, move.TargetPosition);
        }

        public bool IsSolved()
        {
            int expectedValue = 1;

            for (int row = 0; row < rows; row++)
            {
                for (int column = 0; column < columns; column++)
                {
                    if (cells[row, column] == EmptyCell)
                    {
                        continue;
                    }

                    if (cells[row, column] != expectedValue)
                    {
                        return false;
                    }

                    expectedValue++;
                }
            }

            return expectedValue == rows * columns - emptySpaces + 1;
        }

        public bool TryMove(GridPosition sourcePosition, GridDirection direction, out int movedValue, out GridPosition targetPosition, out GridPosition previousPosition)
        {
            movedValue = EmptyCell;
            targetPosition = default;
            previousPosition = default;

            ValidatePosition(sourcePosition);

            targetPosition = GetTargetPositionFromSource(
                sourcePosition,
                direction);

            if (!IsValidPosition(targetPosition))
            {
                return false;
            }

            if (cells[sourcePosition.Row, sourcePosition.Column] == EmptyCell)
            {
                return false;
            }

            if (cells[targetPosition.Row, targetPosition.Column] != EmptyCell)
            {
                return false;
            }

            movedValue = cells[sourcePosition.Row, sourcePosition.Column];

            previousPosition = sourcePosition;

            cells[targetPosition.Row, targetPosition.Column] = movedValue;
            cells[sourcePosition.Row, sourcePosition.Column] = EmptyCell;

            ReplaceEmptyPosition(targetPosition, sourcePosition);

            return true;
        }
        private GridPosition GetTargetPositionFromSource(GridPosition sourcePosition, GridDirection direction)
        {
            switch (direction)
            {
                case GridDirection.Up:
                    return new GridPosition(
                        sourcePosition.Row - 1,
                        sourcePosition.Column);

                case GridDirection.Down:
                    return new GridPosition(
                        sourcePosition.Row + 1,
                        sourcePosition.Column);

                case GridDirection.Left:
                    return new GridPosition(
                        sourcePosition.Row,
                        sourcePosition.Column - 1);

                case GridDirection.Right:
                    return new GridPosition(
                        sourcePosition.Row,
                        sourcePosition.Column + 1);

                default:
                    throw new ArgumentOutOfRangeException(nameof(direction));
            }
        }

        private void InitializeSolved()
        {
            int value = 1;

            emptyPositions.Clear();

            for (int row = 0; row < rows; row++)
            {
                for (int column = 0; column < columns; column++)
                {
                    if (value > rows * columns - emptySpaces)
                    {
                        cells[row, column] = EmptyCell;

                        emptyPositions.Add(new GridPosition(row, column));

                        continue;
                    }

                    cells[row, column] = value;

                    value++;
                }
            }
        }

        private bool TryGetMovePositions(GridDirection direction, out GridPosition sourcePosition, out GridPosition emptyPosition)
        {
            sourcePosition = default;
            emptyPosition = default;

            for (int i = 0; i < emptyPositions.Count; i++)
            {
                GridPosition currentEmpty = emptyPositions[i];

                GridPosition candidatePosition = GetTargetPosition(currentEmpty, direction);

                if (!IsValidPosition(candidatePosition))
                {
                    continue;
                }

                if (cells[candidatePosition.Row, candidatePosition.Column] == EmptyCell)
                {
                    continue;
                }

                sourcePosition = candidatePosition;
                emptyPosition = currentEmpty;

                return true;
            }

            return false;
        }

        private GridPosition GetTargetPosition(GridPosition emptyPosition, GridDirection direction)
        {
            switch (direction)
            {
                case GridDirection.Up:
                    return new GridPosition(
                        emptyPosition.Row - 1,
                        emptyPosition.Column);

                case GridDirection.Down:
                    return new GridPosition(
                        emptyPosition.Row + 1,
                        emptyPosition.Column);

                case GridDirection.Left:
                    return new GridPosition(
                        emptyPosition.Row,
                        emptyPosition.Column - 1);

                case GridDirection.Right:
                    return new GridPosition(
                        emptyPosition.Row,
                        emptyPosition.Column + 1);

                default:
                    throw new ArgumentOutOfRangeException(
                        nameof(direction));
            }
        }

        private void ReplaceEmptyPosition(GridPosition oldPosition, GridPosition newPosition)
        {
            int index = emptyPositions.IndexOf(oldPosition);

            if (index < 0)
            {
                throw new InvalidOperationException("Empty position could not be found.");
            }

            emptyPositions[index] = newPosition;
        }

        private bool IsValidPosition(GridPosition position)
        {
            return position.Row >= 0 &&
                   position.Row < rows &&
                   position.Column >= 0 &&
                   position.Column < columns;
        }

        private void ValidatePosition(GridPosition position)
        {
            if (!IsValidPosition(position))
            {
                throw new ArgumentOutOfRangeException(nameof(position));
            }
        }
    }
}