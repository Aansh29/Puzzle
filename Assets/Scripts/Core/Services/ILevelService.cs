using Puzzle.Core;

namespace Puzzle.Services
{
    public interface ILevelService
    {
        LevelData GetLevel(int levelNumber);
    }
}