using UnityEngine;

namespace Puzzle.Screens
{
    public sealed class GameplayScreen : ScreenBase
    {
        public override ScreenId ScreenId => ScreenId.Gameplay;

        public override ScreenType ScreenType => ScreenType.Exclusive;
    }
}