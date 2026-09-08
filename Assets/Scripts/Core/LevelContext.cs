namespace Puzzle.Core
{
    public sealed class LevelContext
    {
        public LevelData LevelData { get; }

        public LevelContext(LevelData levelData)
        {
            LevelData = levelData;
        }
    }
}