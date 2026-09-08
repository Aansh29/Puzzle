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
            LevelData lastAvailableLevel = null;

            foreach (LevelData level in levels)
            {
                if (level == null)
                {
                    continue;
                }

                lastAvailableLevel = level;

                if (level.LevelNumber == levelNumber)
                {
                    return level;
                }
            }

            if (lastAvailableLevel != null)
            {
                return lastAvailableLevel;
            }

            throw new InvalidOperationException("No levels are available.");
        }
    }
}