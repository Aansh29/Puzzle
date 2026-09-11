using NaughtyAttributes;
using Puzzle.Core;
using Puzzle.Flow;
using Puzzle.Gameplay.Generation;
using Puzzle.Gameplay.Grid;
using Puzzle.Gameplay.Hints;
using Puzzle.Gameplay.History;
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

            ShuffleLevelGenerator shuffleLevelGenerator = new ShuffleLevelGenerator();

            levelGenerator = shuffleLevelGenerator;

            shuffleLevelGenerator.Generate(
                gridModel,
                5);

            ServiceRegistry.Get<IHintService>().SetSolution(
                shuffleLevelGenerator.ShuffleMoves);

            remainingMoves = 5;
            moveLimit = 5;
            levelCompleted = false;

            gameplayScreen.SetMoves(
                remainingMoves,
                moveLimit);

            gridView.Build(gridModel);
        }

        private void OnEnable()
        {
            if (gridView == null)
            {
                return;
            }

            gridView.DirectionDetected += HandleDirectionDetected;
        }

        private void OnDisable()
        {
            if (gridView == null)
            {
                return;
            }

            gridView.DirectionDetected -= HandleDirectionDetected;
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

            gridModel = new GridModel(levelData.Rows, levelData.Columns, levelData.EmptySpaces);

            boardHistory = new BoardHistory();

            ShuffleLevelGenerator shuffleLevelGenerator = new ShuffleLevelGenerator();

            levelGenerator = shuffleLevelGenerator;

            shuffleLevelGenerator.Generate(gridModel, levelData.MoveLimit);

            ServiceRegistry.Get<IHintService>().SetSolution(shuffleLevelGenerator.ShuffleMoves);

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

            ServiceRegistry.Get<IHintService>().RegisterUndo();

            gridModel.UndoMove(move);

            remainingMoves++;

            gridView.MoveTile(
                move.MovedValue,
                move.PreviousPosition,
                gridModel.Rows,
                gridModel.Columns,
                () =>
                {
                    gameplayScreen.SetMoves(
                        remainingMoves,
                        moveLimit);
                });
        }
        public void Hint()
        {
            if (levelCompleted || gridModel == null || remainingMoves <= 0)
                return;

            IHintService hintService = ServiceRegistry.Get<IHintService>();

            if (hintService.TryGetHint(gridModel, remainingMoves, out GridPosition sourcePosition, out GridDirection direction))
                HandleDirectionDetected(sourcePosition, direction);
        }

        private void HandleDirectionDetected(GridPosition sourcePosition, GridDirection direction)
        {
            if (levelCompleted ||
                gridModel == null ||
                remainingMoves <= 0)
            {
                return;
            }

            bool moved =
                gridModel.TryMove(
                    sourcePosition,
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

            ServiceRegistry.Get<IHintService>().RegisterMove(move);

            remainingMoves--;

            gridView.MoveTile(
                movedValue,
                targetPosition,
                gridModel.Rows,
                gridModel.Columns,
                () =>
                {
                    gameplayScreen.SetMoves(
                        remainingMoves,
                        moveLimit);

                    CheckLevelState();
                });
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

            IGameFlowController flowController =
                ServiceRegistry.Get<IGameFlowController>();

            LevelResult result =
                new LevelResult(
                    outcome,
                    moveLimit - remainingMoves);

            _ = flowController.CompleteLevelAsync(result);
        }
    }
}