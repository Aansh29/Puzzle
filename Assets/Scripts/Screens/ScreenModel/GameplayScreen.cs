using Puzzle.Core;
using System;
using TMPro;
using UnityEngine;

namespace Puzzle.Screens
{
    public sealed class GameplayScreenPayload
    {
        public LevelContext LevelContext { get; }

        public GameplayScreenPayload(LevelContext levelContext)
        {
            LevelContext = levelContext;
        }
    }

    public sealed class GameplayScreen : ScreenBase
    {
        [SerializeField]
        private Gameplay.PuzzleGameManager puzzleGameManager;

        [SerializeField]
        private TMP_Text movesText;

        public override ScreenId ScreenId => ScreenId.Gameplay;

        public override ScreenType ScreenType => ScreenType.Exclusive;

        public override void Initialize(object payload)
        {
            if (payload is not GameplayScreenPayload data)
            {
                throw new ArgumentException($"Invalid payload for {nameof(GameplayScreen)}.");
            }

            puzzleGameManager.Initialize(data.LevelContext);
        }

        public void SetMoves(int remainingMoves, int totalMoves)
        {
            movesText.text = $"{remainingMoves} / {totalMoves}";
        }
    }
}