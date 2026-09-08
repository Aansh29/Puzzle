using System;
using Puzzle.Core;
using UnityEngine;

namespace Puzzle.Services
{
    public sealed class LevelService : ILevelService
    {
        private readonly LevelData[] levels;

        public LevelService(LevelData[] levels)
        {
            this.levels = levels ?? throw new ArgumentNullException(nameof(levels));
        }

        public LevelData GetLevel(int levelNumber)
        {
            foreach (LevelData level in levels)
            {
                if (level != null && level.LevelNumber == levelNumber)
                {
                    return level;
                }
            }

            throw new InvalidOperationException($"Level {levelNumber} could not be found.");
        }
    }
}