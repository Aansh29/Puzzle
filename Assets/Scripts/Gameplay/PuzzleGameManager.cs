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

        [SerializeField]
        private Input.SwipeInputController swipeInputController;

        [Button]
        private void TestInitialize()
        {
            LevelData levelData = ScriptableObject.CreateInstance<LevelData>();

            gridModel = new GridModel(5, 7);

            gridView.Build(gridModel);
        }

        private void OnEnable()
        {

            if (swipeInputController == null)
            {
                Debug.LogError("SwipeInputController reference is NULL");

                return;
            }

            swipeInputController.DirectionDetected += HandleDirectionDetected;
        }

        private void OnDisable()
        {
            if (swipeInputController == null)
            {
                return;
            }

            swipeInputController.DirectionDetected -= HandleDirectionDetected;
        }

        public void Initialize(LevelContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            levelContext = context;

            LevelData levelData = levelContext.LevelData;

            gridModel = new GridModel(
                levelData.Rows,
                levelData.Columns);

            gridView.Build(gridModel);
        }

        private void HandleDirectionDetected(GridDirection direction)
        {
            if (gridModel == null)
            {
                return;
            }

            bool moved =
                gridModel.TryMove(
                    direction,
                    out int movedValue,
                    out GridPosition targetPosition);


            if (!moved)
            {
                return;
            }

            gridView.MoveTile(
                movedValue,
                targetPosition,
                gridModel.Rows,
                gridModel.Columns);
        }
    }
}