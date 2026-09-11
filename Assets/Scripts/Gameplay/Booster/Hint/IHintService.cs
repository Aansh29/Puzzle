using Puzzle.Gameplay.Grid;
using Puzzle.Gameplay.History;
using System.Collections.Generic;

namespace Puzzle.Gameplay.Hints
{
    public interface IHintService
    {
        void SetSolution(IReadOnlyList<BoardMove> shuffleMoves);

        void RegisterMove(BoardMove move);

        void RegisterUndo();

        bool TryGetHint(GridModel gridModel, int remainingMoves, out GridPosition sourcePosition, out GridDirection direction);
    }
}