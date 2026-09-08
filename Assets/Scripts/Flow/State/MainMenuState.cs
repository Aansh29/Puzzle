using System;
using System.Threading.Tasks;
using Puzzle.Screens;

namespace Puzzle.Flow
{
    public sealed class MainMenuState : IGameState
    {
        private readonly IScreenManager screenManager;

        public MainMenuState(IScreenManager screenManager)
        {
            this.screenManager = screenManager ?? throw new ArgumentNullException(nameof(screenManager));
        }

        public async Task OnEnterAsync()
        {
            await screenManager.ShowAsync(ScreenId.MainMenu);
        }

        public async Task OnExitAsync()
        {
            await screenManager.HideAsync(ScreenId.MainMenu);
        }
    }
}