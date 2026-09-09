using Puzzle.Gameplay.Grid;

namespace Puzzle.Gameplay.Hints
{
    public interface IHintService
    {
        bool TryGetHint(GridModel gridModel, out GridPosition sourcePosition, out GridDirection direction);
    }
}