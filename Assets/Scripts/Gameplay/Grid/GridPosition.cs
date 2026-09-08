using System;

namespace Puzzle.Gameplay.Grid
{
    public readonly struct GridPosition : IEquatable<GridPosition>
    {
        public int Row { get; }

        public int Column { get; }

        public GridPosition(int row, int column)
        {
            Row = row;
            Column = column;
        }

        public bool Equals(GridPosition other)
        {
            return Row == other.Row && Column == other.Column;
        }

        public override bool Equals(object obj)
        {
            return obj is GridPosition other && Equals(other);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Row, Column);
        }

        public static bool operator ==(GridPosition left, GridPosition right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(GridPosition left, GridPosition right)
        {
            return !left.Equals(right);
        }
    }
}