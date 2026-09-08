using UnityEngine;

namespace Puzzle.Screens
{
    public sealed class GameplayScreen : ScreenBase
    {
        [SerializeField]
        private Gameplay.PuzzleGameManager puzzleGameManager;

        public override ScreenId ScreenId => ScreenId.Gameplay;

        public override ScreenType ScreenType => ScreenType.Exclusive;

        public void Initialize(Puzzle.Core.LevelContext levelContext)
        {
            puzzleGameManager.Initialize(levelContext);
        }
    }
}