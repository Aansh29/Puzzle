using Puzzle.Gameplay.Grid;
using Puzzle.Gameplay.History;
using System;
using System.Collections.Generic;

namespace Puzzle.Gameplay.Hints
{
    public sealed class HintService : IHintService
    {
        private readonly List<BoardMove> playerMoves;
        private readonly List<BoardMove> deviationMoves;

        private IReadOnlyList<BoardMove> shuffleMoves;

        private int solutionIndex;

        public HintService()
        {
            playerMoves = new List<BoardMove>();
            deviationMoves = new List<BoardMove>();
            solutionIndex = -1;
        }

        public void SetSolution(IReadOnlyList<BoardMove> shuffleMoves)
        {
            this.shuffleMoves = shuffleMoves;

            playerMoves.Clear();
            deviationMoves.Clear();

            solutionIndex = shuffleMoves == null ? -1 : shuffleMoves.Count - 1;
        }

        public void RegisterMove(BoardMove move)
        {
            playerMoves.Add(move);

            RecalculateState();
        }

        public void RegisterUndo()
        {
            if (playerMoves.Count <= 0)
            {
                return;
            }

            playerMoves.RemoveAt(playerMoves.Count - 1);

            RecalculateState();
        }

        public bool TryGetHint(GridModel gridModel, int remainingMoves, out GridPosition sourcePosition, out GridDirection direction)
        {
            sourcePosition = default;
            direction = default;

            if (gridModel == null || remainingMoves <= 0)
            {
                return false;
            }

            if (deviationMoves.Count > 0)
            {
                BoardMove deviationMove = deviationMoves[deviationMoves.Count - 1];

                sourcePosition = deviationMove.TargetPosition;

                direction = GetDirection(
                    deviationMove.TargetPosition,
                    deviationMove.PreviousPosition);

                return true;
            }

            if (shuffleMoves == null || solutionIndex < 0)
            {
                return false;
            }

            BoardMove solutionMove = shuffleMoves[solutionIndex];

            sourcePosition = solutionMove.TargetPosition;

            direction = GetDirection(
                solutionMove.TargetPosition,
                solutionMove.PreviousPosition);

            return true;
        }

        private void RecalculateState()
        {
            solutionIndex = shuffleMoves == null ? -1 : shuffleMoves.Count - 1;

            deviationMoves.Clear();

            for (int i = 0; i < playerMoves.Count; i++)
            {
                BoardMove playerMove = playerMoves[i];

                if (deviationMoves.Count > 0)
                {
                    BoardMove lastDeviation = deviationMoves[deviationMoves.Count - 1];

                    if (IsReverseMove(playerMove, lastDeviation))
                    {
                        deviationMoves.RemoveAt(deviationMoves.Count - 1);
                        continue;
                    }

                    deviationMoves.Add(playerMove);
                    continue;
                }

                if (solutionIndex >= 0 && IsReverseMove(playerMove, shuffleMoves[solutionIndex]))
                {
                    solutionIndex--;
                    continue;
                }

                deviationMoves.Add(playerMove);
            }
        }

        private bool IsReverseMove(BoardMove playerMove, BoardMove originalMove)
        {
            return playerMove.MovedValue == originalMove.MovedValue &&
                   playerMove.PreviousPosition == originalMove.TargetPosition &&
                   playerMove.TargetPosition == originalMove.PreviousPosition;
        }

        private GridDirection GetDirection(GridPosition sourcePosition, GridPosition targetPosition)
        {
            if (targetPosition.Row < sourcePosition.Row)
            {
                return GridDirection.Up;
            }

            if (targetPosition.Row > sourcePosition.Row)
            {
                return GridDirection.Down;
            }

            if (targetPosition.Column < sourcePosition.Column)
            {
                return GridDirection.Left;
            }

            return GridDirection.Right;
        }
    }
}