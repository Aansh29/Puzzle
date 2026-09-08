using NaughtyAttributes;
using Puzzle.Core;
using Puzzle.Gameplay.Grid;
using System;
using UnityEngine;

namespace Puzzle.Gameplay
{
    public sealed class PuzzleGameManager : MonoBehaviour
    {
        [SerializeField]
        private GridView gridView;

        private GridModel gridModel;

        private LevelContext levelContext;

        [Button]
        private void TestInitialize()
        {
            LevelData levelData = ScriptableObject.CreateInstance<LevelData>();

            gridModel = new GridModel(5, 7);

            gridView.Build(gridModel);
        }

        public void Initialize(LevelContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            levelContext = context;

            LevelData levelData = levelContext.LevelData;

            gridModel = new GridModel(levelData.Rows, levelData.Columns);

            gridView.Build(gridModel);
        }
    }
}