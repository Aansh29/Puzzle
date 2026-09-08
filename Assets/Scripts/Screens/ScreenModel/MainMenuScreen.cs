using UnityEngine;

namespace Puzzle.Screens
{
    public sealed class MainMenuScreen : ScreenBase
    {
        public override ScreenId ScreenId => ScreenId.MainMenu;

        public override ScreenType ScreenType => ScreenType.Exclusive;
    }
}