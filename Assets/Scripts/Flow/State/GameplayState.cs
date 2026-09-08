using System;
using System.Threading.Tasks;
using Puzzle.Screens;

namespace Puzzle.Flow
{
    public sealed class GameplayState : IGameState
    {
        private readonly IScreenManager screenManager;
        private IGameFlowController flowController;

        public GameplayState(IScreenManager screenManager)
        {
            this.screenManager = screenManager ?? throw new ArgumentNullException(nameof(screenManager));
        }

        public void SetFlowController(IGameFlowController flowController)
        {
            this.flowController = flowController ?? throw new ArgumentNullException(nameof(flowController));
        }

        public async Task OnEnterAsync()
        {
            GameplayScreenPayload screenPayload = new GameplayScreenPayload(flowController.CurrentLevelContext);

            await screenManager.ShowAsync(ScreenId.Gameplay, screenPayload);
        }

        public async Task OnExitAsync()
        {
            await screenManager.HideAsync(ScreenId.Gameplay);
        }
    }
}