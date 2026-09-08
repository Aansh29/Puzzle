using Puzzle.Gameplay.Grid;

namespace Puzzle.Gameplay.Generation
{
    public interface ILevelGenerator
    {
        void Generate(GridModel gridModel, int moveCount);
    }
}