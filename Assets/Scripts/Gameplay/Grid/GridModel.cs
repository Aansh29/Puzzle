using Puzzle.Gameplay.History;
using System;

namespace Puzzle.Gameplay.Grid
{
    public sealed class GridModel
    {
        public const int EmptyCell = -1;

        private readonly int rows;
        private readonly int columns;
        private readonly int[,] cells;

        private GridPosition emptyPosition;

        public int Rows => rows;

        public int Columns => columns;

        public GridPosition EmptyPosition => emptyPosition;

        public GridModel(int rows, int columns)
        {
            if (rows <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(rows));
            }

            if (columns <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(columns));
            }

            this.rows = rows;
            this.columns = columns;

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
            targetPosition = emptyPosition;
            previousPosition = emptyPosition;

            GridPosition sourcePosition = GetTargetPosition(direction);

            if (!IsValidPosition(sourcePosition))
            {
                return false;
            }

            if (cells[sourcePosition.Row, sourcePosition.Column] == EmptyCell)
            {
                return false;
            }

            movedValue = cells[sourcePosition.Row, sourcePosition.Column];

            previousPosition = sourcePosition;

            targetPosition = emptyPosition;

            cells[emptyPosition.Row, emptyPosition.Column] = movedValue;

            cells[sourcePosition.Row, sourcePosition.Column] = EmptyCell;

            emptyPosition = sourcePosition;

            return true;
        }
        public void UndoMove(BoardMove move)
        {
            cells[move.TargetPosition.Row, move.TargetPosition.Column] = EmptyCell;

            cells[move.PreviousPosition.Row, move.PreviousPosition.Column] = move.MovedValue;

            emptyPosition = move.TargetPosition;
        }

        public bool IsSolved()
        {
            int expectedValue = 1;

            for (int row = 0; row < rows; row++)
            {
                for (int column = 0; column < columns; column++)
                {
                    if (row == rows - 1 &&
                        column == columns - 1)
                    {
                        return cells[row, column] == EmptyCell;
                    }

                    if (cells[row, column] != expectedValue)
                    {
                        return false;
                    }

                    expectedValue++;
                }
            }

            return true;
        }

        public int[,] CreateSnapshot()
        {
            int[,] snapshot = new int[rows, columns];

            Array.Copy(
                cells,
                snapshot,
                cells.Length);

            return snapshot;
        }

        public void RestoreSnapshot(int[,] snapshot)
        {
            if (snapshot == null)
            {
                throw new ArgumentNullException(nameof(snapshot));
            }

            if (snapshot.GetLength(0) != rows ||
                snapshot.GetLength(1) != columns)
            {
                throw new ArgumentException("Snapshot dimensions do not match the grid.");
            }

            Array.Copy(
                snapshot,
                cells,
                cells.Length);

            FindEmptyPosition();
        }

        private void InitializeSolved()
        {
            int value = 1;

            for (int row = 0; row < rows; row++)
            {
                for (int column = 0; column < columns; column++)
                {
                    if (row == rows - 1 && column == columns - 1)
                    {
                        cells[row, column] = EmptyCell;

                        emptyPosition = new GridPosition(row, column);

                        continue;
                    }

                    cells[row, column] = value;
                    value++;
                }
            }
        }

        public bool CanMove(GridDirection direction)
        {
            GridPosition sourcePosition = GetTargetPosition(direction);

            return IsValidPosition(sourcePosition) && cells[sourcePosition.Row, sourcePosition.Column] != EmptyCell;
        }

        private GridPosition GetTargetPosition(GridDirection direction)
        {
            switch (direction)
            {
                case GridDirection.Up:
                    return new GridPosition(
                        emptyPosition.Row + 1,
                        emptyPosition.Column);

                case GridDirection.Down:
                    return new GridPosition(
                        emptyPosition.Row - 1,
                        emptyPosition.Column);

                case GridDirection.Left:
                    return new GridPosition(
                        emptyPosition.Row,
                        emptyPosition.Column + 1);

                case GridDirection.Right:
                    return new GridPosition(
                        emptyPosition.Row,
                        emptyPosition.Column - 1);

                default:
                    throw new ArgumentOutOfRangeException(nameof(direction));
            }
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

        private void FindEmptyPosition()
        {
            for (int row = 0; row < rows; row++)
            {
                for (int column = 0; column < columns; column++)
                {
                    if (cells[row, column] == EmptyCell)
                    {
                        emptyPosition = new GridPosition(row, column);

                        return;
                    }
                }
            }

            throw new InvalidOperationException("Grid does not contain an empty cell.");
        }
    }
}