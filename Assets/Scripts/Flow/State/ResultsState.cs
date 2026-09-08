using System;
using System.Threading.Tasks;
using Puzzle.Core;
using Puzzle.Popups;

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