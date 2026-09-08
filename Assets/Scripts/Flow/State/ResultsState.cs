using Puzzle.Core;
using Puzzle.Popups;
using Puzzle.Services;
using System;
using System.Threading.Tasks;

namespace Puzzle.Flow
{
    public sealed class ResultsState : IGameState
    {
        private readonly IPopupManager popupManager;
        private IGameFlowController flowController;

        public ResultsState(IPopupManager popupManager)
        {
            this.popupManager = popupManager ?? throw new ArgumentNullException(nameof(popupManager));
        }
        public void SetFlowController(IGameFlowController flowController)
        {
            this.flowController = flowController ?? throw new ArgumentNullException(nameof(flowController));
        }

        public Task OnEnterAsync()
        {
            LevelResult levelResult = flowController.CurrentLevelResult;

            if (flowController.CurrentLevelResult.Outcome == LevelOutcome.Win)
            {
                ISaveService saveService = ServiceRegistry.Get<ISaveService>();

                int currentLevel = saveService.LoadInt("CurrentLevel");

                saveService.SaveInt("CurrentLevel", currentLevel + 1);
            }

            popupManager.ShowResultPopup(
                levelResult,
                () => true,
                result =>
                {
                    if (result != PopupResult.Accepted)
                    {
                        return;
                    }

                    _ = flowController.QuitToMainMenuAsync();
                });

            return Task.CompletedTask;
        }

        public Task OnExitAsync()
        {
            popupManager.HideAll();

            return Task.CompletedTask;
        }
    }
}