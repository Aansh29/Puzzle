using NaughtyAttributes;
using Puzzle.Core;
using Puzzle.Flow;
using Puzzle.Gameplay.Grid;
using Puzzle.Gameplay.History;
using Puzzle.Gameplay.Generation;
using Puzzle.Services;
using System;
using UnityEngine;

namespace Puzzle.Gameplay
{
    public sealed class PuzzleGameManager : MonoBehaviour
    {
        [SerializeField]
        private GridView gridView;

        [SerializeField]
        private Screens.GameplayScreen gameplayScreen;

        [SerializeField]
        private Input.SwipeInputController swipeInputController;

        private GridModel gridModel;

        private LevelContext levelContext;

        private BoardHistory boardHistory;

        private ILevelGenerator levelGenerator;

        private int remainingMoves;

        private int moveLimit;

        private bool levelCompleted;

        [Button]
        private void TestInitialize()
        {
            gridModel = new GridModel(3, 3);
            boardHistory = new BoardHistory();
            levelGenerator = new ShuffleLevelGenerator();

            levelGenerator.Generate(
                gridModel,
                5);
            remainingMoves = 5;
            gameplayScreen.SetMoves(remainingMoves, 5);

            gridView.Build(gridModel);
        }

        private void OnEnable()
        {
            if (swipeInputController == null)
            {
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

            remainingMoves = levelData.MoveLimit;

            moveLimit = levelData.MoveLimit;

            levelCompleted = false;

            gridModel = new GridModel(levelData.Rows, levelData.Columns);

            boardHistory = new BoardHistory();

            levelGenerator = new ShuffleLevelGenerator();

            levelGenerator.Generate(
                gridModel,
                levelData.MoveLimit);

            gridView.Build(gridModel);

            gameplayScreen.SetMoves(
                remainingMoves,
                moveLimit);
        }

        public void Undo()
        {
            if (levelCompleted ||
                gridModel == null ||
                boardHistory == null ||
                !boardHistory.CanUndo)
            {
                return;
            }

            BoardMove move = boardHistory.Undo();

            gridModel.UndoMove(move);

            remainingMoves++;

            gridView.MoveTile(
                move.MovedValue,
                move.PreviousPosition,
                gridModel.Rows,
                gridModel.Columns);

            gameplayScreen.SetMoves(
                remainingMoves,
                moveLimit);
        }

        private void HandleDirectionDetected(GridDirection direction)
        {
            if (levelCompleted || gridModel == null || remainingMoves <= 0)
            {
                return;
            }

            bool moved =
                gridModel.TryMove(
                    direction,
                    out int movedValue,
                    out GridPosition targetPosition,
                    out GridPosition previousPosition);

            if (!moved)
            {
                return;
            }

            BoardMove move = new BoardMove(movedValue, previousPosition, targetPosition);

            boardHistory.Save(move);

            remainingMoves--;

            gridView.MoveTile(
                movedValue,
                targetPosition,
                gridModel.Rows,
                gridModel.Columns);

            gameplayScreen.SetMoves(
                remainingMoves,
                moveLimit);

            CheckLevelState();
        }

        private void CheckLevelState()
        {
            if (gridModel.IsSolved())
            {
                CompleteLevel(LevelOutcome.Win);
                return;
            }

            if (remainingMoves <= 0)
            {
                CompleteLevel(LevelOutcome.Lose);
            }
        }

        private void CompleteLevel(LevelOutcome outcome)
        {
            if (levelCompleted)
            {
                return;
            }

            levelCompleted = true;

            IGameFlowController flowController = ServiceRegistry.Get<IGameFlowController>();

            LevelResult result = new LevelResult(outcome, moveLimit - remainingMoves);

            _ = flowController.CompleteLevelAsync(result);
        }
    }
}