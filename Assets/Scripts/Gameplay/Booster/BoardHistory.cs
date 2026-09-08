using Puzzle.Gameplay.Grid;
using System.Collections.Generic;

namespace Puzzle.Gameplay.History
{
    public readonly struct BoardMove
    {
        public int MovedValue { get; }

        public GridPosition PreviousPosition { get; }

        public GridPosition TargetPosition { get; }

        public BoardMove(int movedValue, GridPosition previousPosition, GridPosition targetPosition)
        {
            MovedValue = movedValue;
            PreviousPosition = previousPosition;
            TargetPosition = targetPosition;
        }
    }

    public sealed class BoardHistory
    {
        private readonly Stack<BoardMove> moves = new();

        public bool CanUndo => moves.Count > 0;

        public void Save(BoardMove move)
        {
            moves.Push(move);
        }

        public BoardMove Undo()
        {
            return moves.Pop();
        }

        public void Clear()
        {
            moves.Clear();
        }
    }
}