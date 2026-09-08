using System;
using System.Threading.Tasks;
using Puzzle.Screens;

namespace Puzzle.Flow
{
    public sealed class GameplayState : IGameState
    {
        private readonly IScreenManager screenManager;

        public GameplayState(IScreenManager screenManager)
        {
            this.screenManager = screenManager ?? throw new ArgumentNullException(nameof(screenManager));
        }

        public async Task OnEnterAsync()
        {
            await screenManager.ShowAsync(ScreenId.Gameplay);
        }

        public async Task OnExitAsync()
        {
            await screenManager.HideAsync(ScreenId.Gameplay);
        }
    }
}